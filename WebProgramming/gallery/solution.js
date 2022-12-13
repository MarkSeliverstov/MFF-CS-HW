function preprocessGalleryData(imgData)
{
	/*
	 * Your code goes here...
	 */
	
	let groupedBySimilar = imgData.reduce((acc, obj) => {
		let foundGroup = acc.find(objGroup => objGroup.some(i => i.similar.includes(obj)));
		if (foundGroup) {
			foundGroup.push(obj);
		} else {
			acc.push([obj]);
		}
		return acc;
	}, []);
	
	let sortedByDate = groupedBySimilar.map(group => {
		return group.sort((a, b) => {
			return a.created.getTime() - b.created.getTime();
		});
	});

	return sortedByDate;
}

// In nodejs, this is the way how export is performed.
// In browser, module has to be a global varibale object.
module.exports = { preprocessGalleryData };
