
var modName = "HTML";
var modVer = "2.0";
var checkFeature = document.implementation.hasFeature(modName,modVer);
window.document.getElementById("docImplementation").innerHTML = "DOM " + modName + " " + modVer + " поддерживается? " + checkFeature;
