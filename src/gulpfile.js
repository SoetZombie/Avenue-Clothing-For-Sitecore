var gulp = require("gulp");
var msbuild = require("gulp-msbuild");
var debug = require("gulp-debug");
var foreach = require("gulp-foreach");
var rename = require("gulp-rename");
var watch = require("gulp-watch");
var merge = require("merge-stream");
var newer = require("gulp-newer");
var util = require("gulp-util");
var runSequence = require("run-sequence");
var path = require("path");
var config = require("./gulp-config.js")();
var nugetRestore = require('gulp-nuget-restore');
var fs = require('fs');
var unicorn = require("./scripts/unicorn.js");
var habitat = require("./scripts/habitat.js");

module.exports.config = config;

gulp.task("default", function (callback) {
	config.runCleanBuilds = true;
	return runSequence(
	  "00-Copy-Sitecore-Lib",
	  "01-Nuget-Restore",
	  "02-Copy-Serialization-Configuration",
	  "03-Publish-Solution",
	  //"04-Apply-Xml-Transform",
	  "05-Sync-Unicorn",
	  callback);
});

/*****************************
  Initial setup
*****************************/
gulp.task("00-Copy-Sitecore-Lib", function () {
	console.log("Copying Sitecore Libraries");

	fs.statSync(config.sitecoreLibraries);

	var files = config.sitecoreLibraries + "/**/*";

	var libs = gulp.src(files).pipe(gulp.dest("./../lib/Sitecore"));

	return merge(libs);
});

gulp.task("01-Copy-Sitecore-Lib", function () {
	console.log("Copying Sitecore Libraries and License file");

	fs.statSync(config.sitecoreLibraries);

	var files = config.sitecoreLibraries + "/**/*";

	var libs = gulp.src(files).pipe(gulp.dest("./lib/Sitecore"));
	var license = gulp.src(config.licensePath).pipe(gulp.dest("./lib"));

	return merge(libs, license);
});

gulp.task("01-Nuget-Restore", function (callback) {
	var solution = "./" + config.solutionName + ".sln";
	return gulp.src(solution).pipe(nugetRestore());
});

gulp.task("02-Copy-Serialization-Configuration", function (callback) {
	console.log("Copying configuration for serialization to website");
	
	var files = "scripts/Serialization/**/*";
	var libs = gulp.src(files, { base: './scripts/Serialization' }).pipe(gulp.dest(config.websiteRoot));

	return merge(libs);
});

gulp.task("03-Publish-Solution", function (callback) {
	return runSequence(
	  "Build-Solution",
	  "Publish-Solution", callback);
});

gulp.task("04-Apply-Xml-Transform", function () {
	var layerPathFilters = ["./AvenueClothing.B2C/**/*.transform", "!./**/obj/**/*.transform", "!./**/bin/**/*.transform"];
	return gulp.src(layerPathFilters)
   .pipe(foreach(function (stream, file) {
   	var fileToTransform = file.path.replace(/.+AvenueClothing.B2C\\(.+)\.transform/, "$1");
   	util.log("Applying configuration transform: " + file.path);
   	util.log("fileToTransform: " + fileToTransform);
   	return gulp.src("./applytransform.targets")
	  .pipe(msbuild({
	  	targets: ["ApplyTransform"],
	  	configuration: config.buildConfiguration,
	  	logCommand: false,
	  	verbosity: "minimal",
	  	stdout: true,
	  	errorOnFail: true,
	  	maxcpucount: 0,
	  	toolsVersion: 14.0,
	  	properties: {
	  		WebConfigToTransform: config.websiteRoot,
	  		TransformFile: file.path,
	  		FileToTransform: fileToTransform
	  	}
	  }));
   }));
});

gulp.task("05-Sync-Unicorn", function (callback) {
	var options = {};
	options.siteHostName = habitat.getSiteUrl();
	options.authenticationConfigFile = config.websiteRoot + "/App_config/Include/Unicorn/Unicorn.UI.config";

	unicorn(function () { return callback() }, options);
});


gulp.task("06-Deploy-Transforms", function () {
	return gulp.src("./src/**/code/**/*.transform")
	    .pipe(gulp.dest(config.websiteRoot + "/temp/transforms"));
});

/*****************************
  Copy assemblies to all local projects
*****************************/
gulp.task("Copy-Local-Assemblies", function () {
	console.log("Copying site assemblies to all local projects");
	var files = config.sitecoreLibraries + "/**/*";

	var root = "./src";
	var projects = root + "/**/code/bin";
	return gulp.src(projects, { base: root })
	  .pipe(foreach(function (stream, file) {
	  	console.log("copying to " + file.path);
	  	gulp.src(files)
		  .pipe(gulp.dest(file.path));
	  	return stream;
	  }));
});

/*****************************
  Publish
*****************************/
var publishSolution = function (location, dest) {
	dest = dest || config.websiteRoot;
	var targets = ["Build"];

	var solution = "./" + config.solutionName + ".sln";
	return gulp.src(solution)
		  .pipe(msbuild({
		  	targets: targets,
		  	configuration: config.buildConfiguration,
		  	logCommand: false,
		  	verbosity: "minimal",
		  	stdout: true,
		  	errorOnFail: true,
		  	maxcpucount: 0,
		  	toolsVersion: 14.0,
		  	properties: {
		  		DeployOnBuild: "true",
		  		DeployDefaultTarget: "WebPublish",
		  		WebPublishMethod: "FileSystem",
		  		DeleteExistingFiles: "false",
		  		publishUrl: dest,
		  		_FindDependencies: "false"
		  	}
	}));
};

gulp.task("Build-Solution", function () {
	var targets = ["Build"];
	if (config.runCleanBuilds) {
		targets = ["Clean", "Build"];
	}
	var solution = "./" + config.solutionName + ".sln";
	return gulp.src(solution)
		.pipe(msbuild({
			targets: targets,
			configuration: config.buildConfiguration,
			logCommand: false,
			verbosity: "minimal",
			stdout: true,
			errorOnFail: true,
			maxcpucount: 0,
			toolsVersion: 14.0
		}));
});

gulp.task("Publish-Solution", function () {
	return publishSolution();
});

gulp.task("Publish-Assemblies", function () {
	var root = "./src";
	var binFiles = root + "/**/code/**/bin/Sitecore.{Feature,Foundation,Habitat}.*.{dll,pdb}";
	var destination = config.websiteRoot + "/bin/";
	return gulp.src(binFiles, { base: root })
	  .pipe(rename({ dirname: "" }))
	  .pipe(newer(destination))
	  .pipe(debug({ title: "Copying " }))
	  .pipe(gulp.dest(destination));
});

gulp.task("Publish-All-Views", function () {
	var root = "./src";
	var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
	var files = "/**/*.cshtml";
	var destination = config.websiteRoot + "\\Views";
	return gulp.src(roots, { base: root }).pipe(
	  foreach(function (stream, file) {
	  	console.log("Publishing from " + file.path);
	  	gulp.src(file.path + files, { base: file.path })
		  .pipe(newer(destination))
		  .pipe(debug({ title: "Copying " }))
		  .pipe(gulp.dest(destination));
	  	return stream;
	  })
	);
});

gulp.task("Publish-All-Configs", function () {
	var root = "./src";
	var roots = [root + "/**/App_Config", "!" + root + "/**/obj/**/App_Config"];
	var files = "/**/*.config";
	var destination = config.websiteRoot + "\\App_Config";
	return gulp.src(roots, { base: root }).pipe(
	  foreach(function (stream, file) {
	  	console.log("Publishing from " + file.path);
	  	gulp.src(file.path + files, { base: file.path })
		  .pipe(newer(destination))
		  .pipe(debug({ title: "Copying " }))
		  .pipe(gulp.dest(destination));
	  	return stream;
	  })
	);
});

/*****************************
 Watchers
*****************************/
gulp.task("Auto-Publish-Css", function () {
	var root = "./src";
	var roots = [root + "/**/styles", "!" + root + "/**/obj/**/styles"];
	var files = "/**/*.css";
	var destination = config.websiteRoot + "\\styles";
	gulp.src(roots, { base: root }).pipe(
	  foreach(function (stream, rootFolder) {
	  	gulp.watch(rootFolder.path + files, function (event) {
	  		if (event.type === "changed") {
	  			console.log("publish this file " + event.path);
	  			gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
	  		}
	  		console.log("published " + event.path);
	  	});
	  	return stream;
	  })
	);
});

gulp.task("Auto-Publish-Views", function () {
	var root = "./src";
	var roots = [root + "/**/Views", "!" + root + "/**/obj/**/Views"];
	var files = "/**/*.cshtml";
	var destination = config.websiteRoot + "\\Views";
	gulp.src(roots, { base: root }).pipe(
	  foreach(function (stream, rootFolder) {
	  	gulp.watch(rootFolder.path + files, function (event) {
	  		if (event.type === "changed") {
	  			console.log("publish this file " + event.path);
	  			gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
	  		}
	  		console.log("published " + event.path);
	  	});
	  	return stream;
	  })
	);
});

gulp.task("Auto-Publish-Assemblies", function () {
	var root = "./src";
	var roots = [root + "/**/code/**/bin"];
	var files = "/**/Sitecore.{Feature,Foundation,Habitat}.*.{dll,pdb}";;
	var destination = config.websiteRoot + "/bin/";
	gulp.src(roots, { base: root }).pipe(
	  foreach(function (stream, rootFolder) {
	  	gulp.watch(rootFolder.path + files, function (event) {
	  		if (event.type === "changed") {
	  			console.log("publish this file " + event.path);
	  			gulp.src(event.path, { base: rootFolder.path }).pipe(gulp.dest(destination));
	  		}
	  		console.log("published " + event.path);
	  	});
	  	return stream;
	  })
	);
});