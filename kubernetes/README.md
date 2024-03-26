# Minikube Configuration

Follow the instructions below to get app working in a Kubernetes (minikube) environment.

## Before starting

You should have installed and configured the services below:
- Docker
- Minikube
- Kubectl

## Technologies

- Start minikube running `minikube start` command.
- Run `kubectl config use-context minikube` command.
- Run the following commands:
```
# Point the local Docker daemon to the minikube internal Docker registry
minikube docker-env | Invoke-Expression

# Build app image inside minikibe env
docker-build -t my-fav-movies .

# Push image to minikube
minikube image load my-fav-movies
```

- Root folder run `kubectl apply -f .\kubernetes` or select a specific manifest file for applying changes.
- Execute the following command for starting db and application services in minikube:
```
# Start app service with external IP address
minikube service my-fav-movies-service

# Start db service with external IP address
minikube service db-service
```

- With db-service executing with a external IP created, connect to DB using an DB manager tool and execute sql script found on file `../sql.ini`

Use kubectl commands for checking kubernetes resources state:
```
kubectl describe pod/pod_name

kubectl logs pod_name

kubectl get services

kubectl get pods

kubectl get deployments
```
 
#### References

- [Kubectl documentation](https://kubernetes.io/docs/reference/kubectl/)
- [Minikube documentation](https://minikube.sigs.k8s.io/docs/start/)
- [Medium: Deploying .net into kubernettes](https://sogue.medium.com/deploying-a-net-application-to-kubernetes-using-minikube-locally-b04ded64f7ac)
- [Wemakedevs blog post](https://blog.wemakedevs.org/deploy-a-net-application-on-kubernetes)
- [DigitalOcean tutorial](https://www.digitalocean.com/community/tutorials/how-to-deploy-postgres-to-kubernetes-cluster)
- [Justlearnai article](https://justlearnai.com/deploy-a-postgres-db-on-kubernetes-c4d1fb592e2d)