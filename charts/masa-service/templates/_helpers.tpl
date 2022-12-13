{{/*
Expand the name of the chart.
*/}}
{{- define "masa-service.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
We truncate at 63 chars because some Kubernetes name fields are limited to this (by the DNS naming spec).
If release name contains chart name it will be used as a full name.
*/}}
{{- define "masa-service.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Create chart name and version as used by the chart label.
*/}}
{{- define "masa-service.chart" -}}
{{- printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Common labels
*/}}
{{- define "masa-service.labels" -}}
helm.sh/chart: {{ include "masa-service.chart" . }}
{{ include "masa-service.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels
*/}}
{{- define "masa-service.selectorLabels" -}}
app.kubernetes.io/name: {{ .Release.Name }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Create the name of the service account to use
*/}}
{{- define "masa-service.serviceAccountName" -}}
{{- if .Values.serviceAccount.create }}
{{- default (include "masa-service.fullname" .) .Values.serviceAccount.name }}
{{- else }}
{{- default "default" .Values.serviceAccount.name }}
{{- end }}
{{- end }}



{{- define "masa-service.ingress.annotations" -}}
{{- if .Values.ingress.enabled -}}
kubernetes.io/ingress.class: nginx
kubernetes.io/load-balancer-port: "443"
kubernetes.io/load-balancer-protocol: TERMINATED_HTTPS
nginx.ingress.kubernetes.io/cors-allow-headers: DNT,X-CustomHeader,Keep-Alive,User-Agent,X-Requested-With,If-Modified-Since,Cache-Control,Content-Type,Authorization,elastic-apm-traceparent
nginx.ingress.kubernetes.io/cors-allow-methods: PUT, GET, POST, OPTIONS, DELETE
nginx.ingress.kubernetes.io/cors-allow-origin: '*'
nginx.ingress.kubernetes.io/enable-cors: "true"
nginx.ingress.kubernetes.io/proxy-body-size: 200m
nginx.ingress.kubernetes.io/proxy-buffer-size: 32k
nginx.ingress.kubernetes.io/proxy-buffers: 16 64k
nginx.ingress.kubernetes.io/proxy-connect-timeout: "3600"
nginx.ingress.kubernetes.io/proxy-read-timeout: "300"
nginx.ingress.kubernetes.io/ssl-redirect: "true"
{{- end }}
{{- end }}

{{- define "masa-service.ingress.hosts" -}}
{{- if .Values.ingress.enabled -}}
rules:
- host: {{ .Values.domain.name }}
  http:
    paths:
    - backend:
        service:
          name:   {{ .Release.Name }}
          port:
            number: 80
      path: /
      pathType: ImplementationSpecific
tls:
- hosts:
  - {{ .Values.domain.name }}
  secretName: {{ .Values.domain.secret }}
{{- end -}}
{{- end -}}
