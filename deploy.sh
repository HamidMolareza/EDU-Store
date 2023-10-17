#!/bin/bash

echo "Publish project to 'publish' folder..."
dotnet publish -c Release -o publish

echo "Deploy..."
(cd publish && chabok deploy)

echo "Cleaning..."
rm -r publish

echo "Done. :)"