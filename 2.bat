
    powershell -command "docker rm $(docker stop $(docker ps -a -q --filter ancestor=test002 --format=\"{{.ID}}\"))"
    git pull
    docker build  . -t test002
    docker run test002