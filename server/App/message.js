const CreateMsg =  async (type,cont,err,log) =>
{
    let result = 
    {
        Type : type,
        Content : JSON.stringify(cont),
        ErrorCode : err, 
        Timestamp: Date.now()
    }

    if(log)console.log("msg from server: ",result)
    return result
}

module.exports={CreateMsg}



