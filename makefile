


preview0:
	git pull
	make preview2
	
preview1:
	git pull
	make preview3

preview2:
	docker stop Test002
	docker rm Test002
	make preview3

preview3:
	docker build -t aspnetapp .
	docker run -d -p 8080:80 --name Test002 aspnetapp

	



