function preprocessGalleryData(imgData)
{
	/*
	 * Your code goes here...
	 */
	
	var newImgData = Array(Array());
	imgData.forEach(data => {


		newImgData.forEach(newData => {
			if (newData.includes(data)){
				data.similar.forEach(similar => {
					if (!newData.includes(similar)){
						newData.push(similar);
					}
				});
			}
			newImgData.push([data]);
		});


	});


	return [ imgData ];
}

function RecAddSimilar(imgData, newData, data){
	data.similar.forEach(similar => {
		if (!newData.includes(similar)){
			newData.push(similar);
			RecAddSimilar(imgData, newData, similar);
		}
	});
}

// In nodejs, this is the way how export is performed.
// In browser, module has to be a global varibale object.
module.exports = { preprocessGalleryData };
