
	git pull
    docker build  . -t test002
    docker rm $(docker stop $(docker ps -a -q --filter ancestor=test002 --format="{{.ID}}"))
    docker run test002