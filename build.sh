#!/bin/sh

mono .paket/paket.bootstrapper.exe
mono .paket/paket.exe restore
npm install
./node_modules/.bin/bower install --allow-root
./node_modules/.bin/lessc bjoernerlwein_de/static/css/style.less > bjoernerlwein_de/static/css/style.css
mono packages/FAKE/tools/FAKE.exe buildscripts/release.fsx BuildRelease
cd build
mono bjoernerlwein_de.exe production
