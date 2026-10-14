# PowerShell wrapper to trigger Realtime Socket.IO test events
param (
    [string]$BaseUrl = "http://localhost:3001"
)

$env:BASE_URL = $BaseUrl
node "$PSScriptRoot/test_realtime.js"
