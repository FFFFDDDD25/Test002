

previewreset:
	docker kill Test002
	preview

preview:
	docker build -t aspnetapp .
	docker run -d -p 8080:80 --name Test002 aspnetapp