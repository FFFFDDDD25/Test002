


preview0:
	git pull
	make preview1

preview1:
	docker stop Test002
	docker rm Test002
	make preview2

preview2:
	docker build -t aspnetapp .
	docker run -d -p 8080:80 --name Test002 aspnetapp





