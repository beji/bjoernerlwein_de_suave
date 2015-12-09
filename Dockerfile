FROM frolvlad/alpine-mono
RUN mkdir /app
WORKDIR /app
ADD ./build /app
CMD mono bjoernerlwein_de.exe production
