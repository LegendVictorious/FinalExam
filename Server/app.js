var io = require('socket.io')(process.envPort || 3000);
var shortid = require('shortid');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://username:password@ds261678.mlab.com:61678/quizgame";
var fs = require('fs');

console.log("Server Started");

var dbObj;

io.on('connection', function (socket) {
	MongoClient.connect(url, function (err, client) {
		if (err) throw err;
		dbObj = client.db("quizgame");
		console.log("Connected to database");

		socket.on('send data', function (data) {
			dbObj.collection("RoundData").drop( function (){
				if(err) throw err;
			});
			dbObj.createCollection("RoundData", function (){
				if(err) throw err;
			});
			dbObj.collection("RoundData").save(data, function (err, result) {
				if (err) throw err;
				console.log("Round data saved");
			});
		});

		socket.on('retrieve data', function (data) {
			dbObj.collection("RoundData").findOne({}, function (err, result) {
				if (err) throw err;
				fs.writeFile(data.wrappedString, JSON.stringify(result), function(err) {
					if(err) {
						return console.log(err);
					}				
					console.log("The file was saved!");
				});
			});
		});
	});
});