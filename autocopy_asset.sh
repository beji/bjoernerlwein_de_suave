#!/bin/sh

watch_script_js() {
while inotifywait -e modify -e create -e delete bjoernerlwein_de/static/js/script.js; do
    cp bjoernerlwein_de/static/js/script.js build/static/js/script.js
done;
}

watch_pages(){
inotifywait -m -e modify -e create -e delete --format %f bjoernerlwein_de/content/pages | while read FILE
do
    cp bjoernerlwein_de/content/pages/$FILE build/content/pages/$FILE
done;
}

watch_posts(){
  inotifywait -m -e modify -e create -e delete --format %f bjoernerlwein_de/content/posts | while read FILE
  do
      cp bjoernerlwein_de/content/posts/$FILE build/content/posts/$FILE
  done;  
}
watch_less(){
  while inotifywait -e modify -e create -e delete bjoernerlwein_de/static/css/style.less; do
      lessc bjoernerlwein_de/static/css/style.less > bjoernerlwein_de/static/css/style.css
      cp bjoernerlwein_de/static/css/style.css build/static/css/style.css
  done;
}

watch_script_js &
watch_pages &
watch_posts &
watch_less

