function calcTime(city, offset) {
    // create Date object for current location
    var d = new Date();

    // convert to msec
    // add local time zone offset 
    // get UTC time in msec
    var utc = d.getTime() + (d.getTimezoneOffset() * 60000);

    // create new Date object for different city
    // using supplied offset
    var nd = new Date(utc + (3600000 * offset));

    // return time as a string
    return "The local time in " + city + " is " + nd.toLocaleString() + " - " + d.toLocaleString();

}

//Covert datetime by GMT offset 
//If toUTC is true then return UTC time other wise return local time
function convertLocalDateToUTCDate(date, toUtc) {
    date = new Date(date);
    //Local time converted to UTC
    console.log("Time: " + date);
    var localOffset = date.getTimezoneOffset() * 60000;
    var localTime = date.getTime();
    if (toUtc) {
        date = localTime + localOffset;
    } else {
        date = localTime - localOffset;
    }
    date = new Date(date);
    console.log("Converted time: " + date);
    return date;
}
