var io = require('socket.io')(process.envPort||3000);
var shortid = require('shortid');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://username:password@ds261678.mlab.com:61678/quizgame";

console.log("Server Started");

var dbObj;

MongoClient.connect(url, function(err, client){
	if(err) throw err;
	dbObj = client.db("quizgame");	
	console.log("Connected to database");
});

io.on('connection', function(socket){
	socket.broadcast.emit('retrieve data');

	socket.on('connection made', function(data){
		console.log("Connection made with quiz game");
	});

	socket.on('senddata', function(data){
		console.log("send data");
		console.log(JSON.stringify(data));
		dbObj.collection("RoundData").save(data, function(err, result){
			if(err) throw err;
			console.log("Round data saved");
		});
	});
});