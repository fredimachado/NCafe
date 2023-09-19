# Web App
A base web app helm chart to deploy docker images to Kubernetes

## Add Repo

```console
helm repo add fredi https://gitlab.fredi.au/api/v4/projects/35/packages/helm/stable
helm repo update
```

## Install Chart

```console
helm install [RELEASE_NAME] fredi/web-app
```

The command deploys a simple web-app (nginx) on the Kubernetes cluster in the default configuration.

_See [configuration](#configuration) below._

_See [helm install](https://helm.sh/docs/helm/helm_install/) for command documentation._

## Uninstall Chart

```console
helm uninstall [RELEASE_NAME]
```

This removes all the Kubernetes components associated with the chart and deletes the release.

_See [helm uninstall](https://helm.sh/docs/helm/helm_uninstall/) for command documentation._

## Upgrading Chart

```console
helm upgrade [RELEASE_NAME] fredi/web-app --install
```

_See [helm upgrade](https://helm.sh/docs/helm/helm_upgrade/) for command documentation._

## Configuration

See [Customizing the Chart Before Installing](https://helm.sh/docs/intro/using_helm/#customizing-the-chart-before-installing). To see all configurable options with detailed comments, visit the chart's [values.yaml](./values.yaml), or run these configuration commands:

```console
helm show values fredi/web-app
```
