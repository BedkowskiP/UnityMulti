const mysql = require('mysql');

class database
{
    constructor(hst,usr,pass,dbname)
    {
        this.con = mysql.createConnection(
           {
            host:hst,
            user:usr,
            password:pass,
            database:dbname
           } )
    }
    
    Connect()
    {
        this.con.connect(async(err) => {
            if (err) throw err;
            console.log("Connected!");
        });
    }
    Query(que)
    {
        this.con.query(que, function (err, result, fields) {
        if (err) throw err;
        return result;
        });
        
    }
    Close()
    {
        this.con.end();
    }
    DBSTATE = () => this.con.DBSTATE;
}


    





module.exports = {database}