const logger = require('./../logger')
const CreateMsg =  (type,cont,err,log,broadcast) =>
{
    let result = JSON.stringify(
    {
        Type : type,
        Content : JSON.stringify(cont),
        ErrorCode : err, 
        Timestamp: Date.now()
    })
    
    if(log&&broadcast==null)logger.LogFile(result);
    
    return result
}

module.exports={CreateMsg}



