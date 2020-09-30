

previewreset:
	docker stop Test002
	docker rm Test002
	make preview

preview:
	docker build -t aspnetapp .
	docker run -d -p 8080:80 --name Test002 aspnetapp