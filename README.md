# Visus.VcpkgCache
This application implements an HTTP-based package cache for [vcpkg](https://vcpkg.io/).

## Installation
### Prerequesites
Install ASP.NET Core:
```bash
rpm -Uvh https://packages.microsoft.com/config/rhel/9//packages-microsoft-prod.rpm
dnf update
dnf install -y aspnetcore-runtime-8.0
```

Install nginx as reverse proxy. The application should also work with other proxies, but the instructions below are for nginx.
```bash
dnf install -y nginx
```

For debugging purposes, it might be helpful to install SELinux tools, but this is optional:
```bash
dnf install -y policycoreutils-python
dnf install -y setroubleshoot-server
```

### Create the application user
The instructions assume that the application runs as user `kestrel`:
```bash
adduser kestrel
id kestrel
```

### Deploy the application
Create the application directory and the cache directory:
```bash
mkdir /opt/vcpkgcache
chown -R kestrel:kestrel /opt/vcpkgcache
mkdir /var/vcpkgcache
chown -R kestrel:kestrel /var/vcpkgcache
```

Build and publish the application and copy the published folder to /tmp/publish on the server. From there, copy everything to the application directory and reset the permissions:
```bash
\cp -r /tmp/publish/* /opt/vcpkgcache/
chown -R kestrel:kestrel /opt/vcpkgcache
```

Update the application settings in `/opt/vckpgcache/appsettings.json`:
```json
{
    "CacheSettings": {
        "Path": "/var/vcpkgcache",
        "Token": "<set a reasonable token here>"
    }
    "KestrelLimits": {
        "MaxRequestBodySize": 1073741824
    }
}
```

Create a unit file (or copy [kestrel-vcpkgcache.service](unit-files/kestrel-vcpkgcache.service)) for the application at `/etc/systemd/system/kestrel-vcpkgcache.service` and enable and start the service:
```bash
systemctl enable kestrel-vcpkgcache.service
systemctl start kestrel-vcpkgcache.service
systemctl status kestrel-vcpkgcache.service
```

### Set up the reverse proxy:
In `/etc/nginx/nginx.conf`, replace the `server` section with a new one for the reverse proxy:
```
server {
    listen 80;
    listen [::]:80;

    location ~ (.*) {
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        # Note: make sure that the application is configured to run on this port:
        proxy_pass http://127.0.0.1:5000$1?$args;
    }
}
```

Alternatively, add an HTTPS end point and redirect everything from HTTP to HTTPS:
```
server {
    listen 80;
    listen [::]:80;
    return 301 https://$host$request_uri;
}

server {
    listen 443 ssl;
    listen [::]:443 ssl;

    ssl_certificate "/etc/pki/tls/certs/server.crt";
    ssl_certificate_key "/etc/pki/tls/private/server.key";
    ssl_session_cache shared:SSL:1m;
    ssl_session_timeout 10m;
    ssl_ciphers PROFILE=SYSTEM;
    ssl_prefer_server_ciphers on;

    location ~ (.*) {
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;

        # Note: make sure that the application is configured to run on this port:
        proxy_pass http://127.0.0.1:5010$1?$args;
    }
}
```

Make sure that nginx is allowed to make HTTP requests:
```bash
setsebool -P httpd_can_network_connect 1
```

Enable and start nginx:
```bash
systemctl enable nginx.service
systemctl start nginx.service
systemctl status nginx.service
```

Open the HTTP and/or HTTP ports in the firewall:
```bash
firewall-cmd --permanent --zone=public --add-service=http
firewall-cmd --permanent --zone=public --add-service=https
firewall-cmd --reload
```

## Usage
The application implements [binary caching](https://learn.microsoft.com/en-us/vcpkg/reference/binarycaching#http) by means of the `GET`, `HEAD` and `PUT` verbs.

### Adding a package
Call `PUT` with the "Authorization" header set tp "Token <your secret from appsettings.json>" and send the binary as the body. The path determines the file name, i.e. if you "PUT /x64-windows-embree-f68a0a4952c92aa6363de44910fdf569b3906f6a72ad8fbe7c09551a2d06a261", the data will be stored under the name "x64-windows-embree-f68a0a4952c92aa6363de44910fdf569b3906f6a72ad8fbe7c09551a2d06a261".

### Retrieve a package
Call `GET` on the package name, e.g. "GET /x64-windows-embree-f68a0a4952c92aa6363de44910fdf569b3906f6a72ad8fbe7c09551a2d06a261".

### Check whether a package exists
Call `HEAD` on the package name, e.g. "HEAD /x64-windows-embree-f68a0a4952c92aa6363de44910fdf569b3906f6a72ad8fbe7c09551a2d06a261". If you get a success status code, the package is available. If you get an HTTP 404, the package is not in the cache.

### List packages
You can call `GET /` to retrieve a JSON array of all packages in the cache, provided you send the authorisation header.
