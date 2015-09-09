luanet.load_assembly("Game1");
DEFAULT_WIDTH=50;
DEFAULT_HEIGHT=50;
MAP_WIDTH=50;
MAP_HEIGHT=50;
currentSettings=Simple:Settings();
--currentSettings:x = 0;
--currentSettings:y = 0;

mainHero = Simple:CreateTile("hero","hero.png",50);
mainHero:Resize(DEFAULT_WIDTH,DEFAULT_HEIGHT);
mainHero.passable=false;

gndTile = Simple:CreateTile("gnd","gnd.png");
gndTile:AddImg("gnd.png");
gndTile:AddImg("gnd2.png");
gndTile:AddImg("gnd3.png");
gndTile:Resize(DEFAULT_WIDTH,DEFAULT_HEIGHT);

boxTile = Simple:CreateTile("box","box.png");
boxTile:Resize(DEFAULT_WIDTH,DEFAULT_HEIGHT);
boxTile.passable=false;

mainMap = Simple:CreateMap("main",gndTile,MAP_WIDTH,MAP_HEIGHT);
mainMap:AddObject(boxTile,10,12);
mainMap:AddObject(boxTile,10,11);
mainMap:AddObject(boxTile,10,10);
mainMap:AddObject(boxTile,10,8);

heroActor = Simple:CreateActor("Hero",mainHero,300,300);
heroActor:StartAnim();