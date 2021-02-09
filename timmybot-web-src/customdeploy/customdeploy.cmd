cd ..
winrar a -afzip -r -x*\.vs\ -x*\bin\ -x*\obj\ -x*\customdeploy\ .\customdeploy\code
az webapp deployment source config-zip --resource-group "TimmyBotRG" --name "timmybot-web" --src ".\customdeploy\code.zip"
del ".\customdeploy\code.zip"