var io = require('socket.io')(process.envPort || 4567);
var shortid = require('shortid');
var http = require('http');
var path = require('path');
var express = require('express');
var logger = require('morgan');
var bodyParser = require('body-parser');
var cookieParser = require('cookie-parser');
var passport = require('passport');
var session = require('express-session');
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://username:password@ds261678.mlab.com:61678/quizgame";
var fs = require('fs');

console.log("Server Started");

//-----------------------------------------------------------------------------------------
// Socket.IO / Unity interface
io.on('connection', function (socket) {
	var dbObj;

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

		socket.on('send score data', function (data) {
			dbObj.collection("ScoreData").save(data, function (err, result) {
				if (err) throw err;
				console.log("Score data saved");
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

		socket.on('retrieve score data', function (data) {
			dbObj.collection("ScoreData").find({}).toArray(function(err, result){
				if (err) throw err;
				result = "{ \"scoreDataArray\":" + JSON.stringify(result) + "}";
				fs.writeFile(data.wrappedString, result, function(err) {
					if(err) {
						return console.log(err);
					}				
					console.log("The score data was saved!");
				});
			});
		});
	});
});
//-----------------------------------------------------------------------------------------

// Website
var app = express();

app.set('views', path.resolve(__dirname, 'views'));
app.set('view engine', 'ejs');

app.use(logger("dev"));

app.use(bodyParser.urlencoded({extended:false}));
app.use(cookieParser());
app.use(session({
	secret:"secretSession",
	resave:true,
	saveUninitialized:true
}));

app.use(passport.initialize());
app.use(passport.session());

passport.serializeUser(function(user, done){
	done(null, user);
});

passport.deserializeUser(function(user, done){
	done(null, user);
});

LocalStrategy = require('passport-local').Strategy;

passport.use(new LocalStrategy({
	usernameField:'',
	passwordField:''
	},
	function(username, password, done){
		MongoClient.connect(url, function(err, db){
			if(err)throw err;
			
			var dbObj = db.db("quizgame");
			
			dbObj.collection("users").findOne({username:username}, function(err,results){
				if(results.password == password){
					var user = results;
					done(null, user);
				}
				else{
					done(null, false, {message:'Bad Password'});
				}
			});
		});
		
}));

function ensureAuthenticated(req, res, next){
	if(req.isAuthenticated()){
		next();
	}
	else{
		res.redirect("/sign-in");
	}
}

app.get('/logout', function(req, res){
	req.logout();
	res.redirect("/sign-in");
});

app.get("/", ensureAuthenticated, function(request,response){
	MongoClient.connect(url, function(err,db){
		if(err)throw err;
		var dbObj= db.db("quizgame");
		
		dbObj.collection("RoundData").findOne({}, function(err, results){
			console.log("Site Served");
			db.close();
			response.render("index", {roundData:results});
		});		
	});	
});

function CompareScores(score1, score2){
	if (score1.playerScore < score2.playerScore)
    return 1;
  	if (score1.playerScore > score2.playerScore)
    return -1;
  return 0;
}

app.get("/high-scores",ensureAuthenticated, function(request,response){
	MongoClient.connect(url, function(err,db){
		if(err)throw err;
		var dbObj= db.db("quizgame");
		
		dbObj.collection("ScoreData").find({}).toArray(function(err, results){
			db.close();
			results.sort(CompareScores);
			response.render("high-scores", {allScoreData:results});
		});		
	});
});

app.get("/sign-in", function(request,response){
	response.render("sign-in");
});

app.post("/sign-up", function(request,response){
	console.log(request.body);
	MongoClient.connect(url, function(err, db){
	if(err)throw err;
	var dbObj = db.db("quizgame");
		
	var user = {
		username: request.body.username,
		password: request.body.password
	}
	
	dbObj.collection('users').insert(user,function(err, results){
			if(err)throw err;
			
				request.login(request.body, function(){
				response.redirect('/');
			});
		});
	});
	
});

app.post("/sign-in", passport.authenticate('local', {
	failureRedirect:'/sign-in'
	}), function(request,response){
			response.redirect('/');
});

app.get('/profile', function(request,response){
		response.json(request.user);
});
app.use(function(request, response){
	response.status(404).render("404");
});

http.createServer(app).listen(3000, function(){
	console.log("Quiz data server started on port 3000");
});