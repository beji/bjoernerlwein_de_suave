#!/bin/sh

mono .paket/paket.bootstrapper.exe
mono .paket/paket.exe install
mono packages/FAKE/tools/FAKE.exe buildscripts/release.fsx BuildRelease
cd build
mono bjoernerlwein_de.exe production
