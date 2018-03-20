# simple-desambiguation-bot

## README In Construction



### Requeriments
* Visual Studio 2015 or higher
* .NET Framework


### How do Run
* Create a luis service in azure and upload the ```SimpleBot.json``` file
* Fill your credentials in the file ```Dialogs/Base/BaseLuisScoreDialog.cs```


### Example Questions

* High Threshold

    "How do I get a card"
    
    "How do I create an account"
    
* Medium Threshold

    "how do i create a personal card"
    
* Low Threshold

    "banana"
