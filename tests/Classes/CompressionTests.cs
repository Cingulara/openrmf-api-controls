// Copyright (c) Cingulara LLC 2025 and Tutela LLC 2025. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license.

using System;
using System.Text;
using Xunit;
using openrmf_api_controls.Classes;

namespace tests.Classes
{
    public class CompressionTests
    {
        // ─── PASS: CompressString produces non-empty output ───────────────────────

        [Fact]
        public void Test_CompressString_IsNotNull()
        {
            var result = Compression.CompressString("hello world");
            Assert.NotNull(result);
        }

        [Fact]
        public void Test_CompressString_IsNotEmpty()
        {
            var result = Compression.CompressString("hello world");
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Test_CompressString_ProducesBase64String()
        {
            var result = Compression.CompressString("hello world");
            // Should not throw – valid Base64 string produced
            var bytes = Convert.FromBase64String(result);
            Assert.NotNull(bytes);
        }

        // ─── PASS: round-trip compress → decompress returns original ─────────────

        [Fact]
        public void Test_CompressDecompress_RoundTrip_SimpleString()
        {
            const string original = "hello world";
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Test_CompressDecompress_RoundTrip_EmptyString()
        {
            const string original = "";
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Test_CompressDecompress_RoundTrip_JsonPayload()
        {
            const string original = "[{\"number\":\"AC-1\",\"title\":\"Access Control\",\"lowimpact\":true}]";
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Test_CompressDecompress_RoundTrip_LargeString()
        {
            var original = new string('A', 10_000);
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        [Fact]
        public void Test_CompressDecompress_RoundTrip_SpecialCharacters()
        {
            const string original = "!@#$%^&*()_+\t\n\r Unicode: \u4e2d\u6587 \u00e9\u00e0\u00fc";
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(original, decompressed);
        }

        // ─── PASS: compressed output is smaller than a repetitive input ───────────

        [Fact]
        public void Test_CompressString_RepetitiveData_ShorterThanOriginal()
        {
            var original = new string('Z', 5000);
            var compressed = Compression.CompressString(original);
            // Base64 overhead is ~1.33x; still significantly shorter than raw 5000 chars
            Assert.True(compressed.Length < original.Length,
                $"Compressed length {compressed.Length} should be < original {original.Length}");
        }

        // ─── PASS: compressed output differs from original ────────────────────────

        [Fact]
        public void Test_CompressString_OutputDiffersFromInput()
        {
            const string original = "some test data";
            var compressed = Compression.CompressString(original);
            Assert.NotEqual(original, compressed);
        }

        // ─── FAIL: invalid Base64 passed to DecompressString throws ──────────────

        [Fact]
        public void Test_DecompressString_InvalidBase64_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() =>
                Compression.DecompressString("not-valid-base64!!!"));
        }

        [Fact]
        public void Test_DecompressString_InvalidGzipBytes_ThrowsException()
        {
            // First 4 bytes = length header (4), next 4 bytes = not valid gzip data.
            // DecompressString will attempt GZip decompression and throw InvalidDataException.
            var payload = new byte[] { 0x04, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
            var invalidPayload = Convert.ToBase64String(payload);
            Assert.ThrowsAny<Exception>(() =>
                Compression.DecompressString(invalidPayload));
        }

        // ─── FAIL: decompressed value is NOT the compressed string ────────────────

        [Fact]
        public void Test_CompressDecompress_DecompressedIsNotTheCompressedString()
        {
            const string original = "hello world";
            var compressed = Compression.CompressString(original);
            var decompressed = Compression.DecompressString(compressed);
            Assert.NotEqual(compressed, decompressed);
        }

        // ─── THEORY: multiple strings round-trip correctly ───────────────────────

        [Theory]
        [InlineData("AC-1")]
        [InlineData("SI-12")]
        [InlineData("{\"impactLevel\":\"high\",\"pii\":false}")]
        [InlineData("The quick brown fox jumps over the lazy dog")]
        public void Test_CompressDecompress_RoundTrip_Theory(string input)
        {
            var compressed = Compression.CompressString(input);
            var decompressed = Compression.DecompressString(compressed);
            Assert.Equal(input, decompressed);
        }
    }
}
