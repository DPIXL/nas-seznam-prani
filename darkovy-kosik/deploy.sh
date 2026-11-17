#!/usr/bin/env bash
set -e

echo "Publishing Release build..."
dotnet publish -c Release -o ./publish -p:AllowMissingPrunePackageData=true

echo "Syncing changed files to VPS..."
rsync -avz --delete ./publish/ wishlist-vps:/var/www/wishlist/

echo "Restarting wishlist.service..."
ssh wishlist-vps "sudo systemctl restart wishlist.service"

echo "Deployment complete: https://wishlist.dpixl.me"