/**
 * Data model for loading the work hour categories and fileld hours.
 * The model implements internal cache, so the data does not have to be
 * loaded every time from the REST API.
 */
class DataModel {
	/**
	 * Initialize the data model with given URL pointing to REST API.
	 * @param {string} apiUrl Api URL prefix (without the query part).
	 */
	constructor(apiUrl)
	{
		// Your code goes here...
	}


	/**
	 * Retrieve the data and pass them to given callback function.
	 * If the data are available in cache, the callback is invoked immediately (synchronously).
	 * Otherwise the data are loaded from the REST API and cached internally.
	 * @param {Function} callback Function which is called back once the data become available.
	 *                     The callback receives the data (as array of objects, where each object
	 *                     holds `id`, `caption`, and `hours` properties).
	 *                     If the fetch failed, the callback is invoked with two arguments,
	 *                     first one (data) is null, the second one is error message
	 */
	getData(callback)
	{
		// Your code goes here...
	}


	/**
	 * Invalidate internal cache. Next invocation of getData() will be forced to load data from the server.
	 */
	invalidate()
	{
		// Your code goes here...
	}

	
	/**
	 * Modify hours for one record.
	 * @param {number} id ID of the record in question.
	 * @param {number} hours New value of the hours (m)
	 * @param {Function} callback Invoked when the operation is completed.
	 *                            On failutre, one argument with error message is passed to the callback.
	 */
	setHours(id, hours, callback = null)
	{
		// Your code goes here...
	}
}


// In nodejs, this is the way how export is performed.
// In browser, module has to be a global varibale object.
module.exports = { DataModel };
