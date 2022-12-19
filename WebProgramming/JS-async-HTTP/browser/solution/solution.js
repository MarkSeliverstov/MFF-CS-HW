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
		this.apiUrl = apiUrl;
		this.cache = [];
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
	async getData(callback)
	{
		// Your code goes here...
		if (this.cache.length > 0) {
			callback(this.cache);
		}
		else {
			try{
				const data = await (await fetch(this.apiUrl)).json();
				if (!data.ok){
					callback(null, data.error);
				}
				else{
					let complete_cache = [];

					for (let i = 0; i < data.payload.length; i++){
						try{
							const item_data =  await (await fetch(
								this.apiUrl + 
								"?action=hours&id=" + 
								data.payload[i].id)).json();
							
								if (!item_data.ok){
									callback(null, item_data.error);
									return;
								}
								else{
									complete_cache.push({
										id: data.payload[i].id,
										caption: data.payload[i].caption,
										hours: item_data.payload.hours
									});
								}
						}
						catch (error){
						}
					}

					this.cache = complete_cache;
					callback(this.cache);
				}
			}
			catch (error){
			}
		}
	}


	/**
	 * Invalidate internal cache. Next invocation of getData() will be forced to load data from the server.
	 */
	invalidate()
	{
		// Your code goes here...
		this.cache = [];
	}

	
	/**
	 * Modify hours for one record.
	 * @param {number} id ID of the record in question.
	 * @param {number} hours New value of the hours (m)
	 * @param {Function} callback Invoked when the operation is completed.
	 *                            On failutre, one argument with error message is passed to the callback.
	 */
	async setHours(id, hours, callback = null)
	{
		// Your code goes here...
		try{
			const data = await (await fetch(
				this.apiUrl +
				"?action=hours&id=" + id +
				"&hours=" + hours,
				{method: "POST"})).json();

			if (!data.ok){
				callback(data.error);
			}
			else{
				await this.invalidate();
				callback();
			}
		}
		catch (error){
		}
	}
}


// In nodejs, this is the way how export is performed.
// In browser, module has to be a global varibale object.
module.exports = { DataModel };
