FROM voidlinux/voidlinux
RUN xbps-install -Sy
RUN xbps-install mono
RUN mkdir /app
WORKDIR /app
ADD ./build /app
CMD mono bjoernerlwein_de.exe production
