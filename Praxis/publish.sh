#!/bin/bash

env

if ["$TRAVIS_BRANCH" == "master"] && [! -z "$TRAVIS_TAG"]; then
  echo "This is a new release, yay!  Pushing nuget package."
  docker-compose run --rm praxis-cli mono /usr/local/bin/nuget push Praxis.$TRAVIS_TAG.nupkg $NUGET_KEY
fi
