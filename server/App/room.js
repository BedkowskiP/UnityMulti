class Room
{
    constructor(name,host,password,isPublic,maxPlayers)
    {
        this.name=name;
        this.host=host
        this.password=password
        this.isPublic=isPublic
        this.maxPlayers=maxPlayers
    } 
}


module.exports = {Room}