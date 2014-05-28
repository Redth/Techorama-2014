function PushExpired() {
    mssql.query('select * from Item where Expires <= GETDATE()',{
        success: function(results) {

            console.log('PushExpired->Results: ' + results.length);

            var azure = require('azure');
            var hub = azure.createNotificationHubService('HUB-NAME','FULL-ACCESS-CONNECTION-STRING');

            for (var i = 0; i < results.length; i++) {

                var item = results[i];
                var payload = '{ "message" : "Your ' + item.Name + ' has expired!" }';

                console.log('PushExpired->Payload: ' + item.UserId + ' -> ' + payload);

                hub.send(item.UserId, payload, function(error, outcome) {
                    console.log('PushExpired->Error: ' + error);
                });
            }
        }
    });
}
