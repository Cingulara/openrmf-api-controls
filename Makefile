VERSION ?= 1.08.01
NAME ?= "openrmf-api-controls"
AUTHOR ?= "Dale Bingham"
PORT_EXT ?= 8094
PORT_INT ?= 8094
NO_CACHE ?= true
DOCKERHUB_ACCOUNT ?= cingulara
  
.PHONY: build latest docker clean version dockerhub

build:  
	dotnet build src

docker: 
	docker build -f Dockerfile -t $(NAME)\:$(VERSION) --no-cache=$(NO_CACHE) .

latest: 
	docker build -f Dockerfile -t $(NAME)\:latest --no-cache=$(NO_CACHE) .
	docker tag $(NAME)\:latest ${DOCKERHUB_ACCOUNT}\/$(NAME)\:latest
	docker push ${DOCKERHUB_ACCOUNT}\/$(NAME)\:latest
   
clean:
	@rm -f -r src/obj
	@rm -f -r src/bin
	@rm -f -r tests/obj
	@rm -f -r tests/bin

version:
	@echo ${VERSION}

dockerhub:
	docker tag $(NAME)\:$(VERSION) ${DOCKERHUB_ACCOUNT}\/$(NAME)\:$(VERSION)
	docker push ${DOCKERHUB_ACCOUNT}\/$(NAME)\:$(VERSION)

DEFAULT: build
