var io = require('socket.io')(process.envPort||3000);
var shortid = require('shortid');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://localhost:27017";

console.log("Server Started");
//console.log(shortid.generate());

var players = [];

var dbObj;

MongoClient.connect(url, function(err, client){
	if(err) throw err;
	dbObj = client.db("SocketGameData");	
	console.log("Connected to database");
});

io.on('connection', function(socket){
	var thisPlayerId = shortid.generate();
	
	players.push(thisPlayerId);
	
	console.log('client connected spawning player id: '+ thisPlayerId);
	
	socket.broadcast.emit('spawn player', {id:thisPlayerId});
	
	players.forEach(function(playerId){
		if(playerId == thisPlayerId)return;
		
		socket.emit('spawn player', {id:playerId});
		console.log("Adding a new player", playerId);
	});
	
	socket.on('move', function(data){
		data.id = thisPlayerId;
		console.log("Player position is: ", JSON.stringify(data));
		socket.broadcast.emit('move', data);
	});
	
	socket.on('disconnect', function(){
		console.log("Player Disconnected");
		players.splice(players.indexOf(thisPlayerId),1);
		socket.broadcast.emit('disconnected', {id:thisPlayerId});
	});

	socket.on('send data', function(data){
		console.log(JSON.stringify(data));
		dbObj.collection("PlayerData").save(data, function(err, result){
			if(err) throw err;
			console.log("Player data saved");
		});
	});
});