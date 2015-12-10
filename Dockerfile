FROM voidlinux/voidlinux
RUN ln -sf /usr/share/zoneinfo/Europe/Berlin /etc/localtime
RUN xbps-install -Suy mono libuv libuv-devel
RUN mkdir /app
WORKDIR /app
ADD ./build /app
CMD mono bjoernerlwein_de.exe production
