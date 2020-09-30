

previewreset:
	docker build -t aspnetapp .
	docker run -d -p 8080:80 --name Test002 aspnetapp

preview:
	docker kill Test002
	docker build -t aspnetapp .
	docker run -d -p 8080:80 --name Test002 aspnetapp