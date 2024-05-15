#!/bin/sh

set -e

APPSETTINGS=/user/share/nginx/html/appsettings.json

# delete compressed (brotli/gzip) appsettings files
rm -rf "$APPSETTINGS.br"
rm -rf "$APPSETTINGS.gz"

# update base address value using environment variables
sed -i -E "s|(\"AdminBaseAddress\"\:) \"(.*)\"(,)?|\1 \"$ADMIN_BASE_ADDRESS\"\3|g" $APPSETTINGS
sed -i -E "s|(\"CashierBaseAddress\"\:) \"(.*)\"(,)?|\1 \"$CASHIER_BASE_ADDRESS\"\3|g" $APPSETTINGS
sed -i -E "s|(\"BaristaBaseAddress\"\:) \"(.*)\"(,)?|\1 \"$BARISTA_BASE_ADDRESS\"\3|g" $APPSETTINGS

cat $APPSETTINGS
