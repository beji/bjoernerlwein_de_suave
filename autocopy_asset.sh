#!/bin/sh

while inotifywait -e modify -e create -e delete bjoernerlwein_de/static/js/script.js; do
    cp bjoernerlwein_de/static/js/script.js build/static/js/script.js
done