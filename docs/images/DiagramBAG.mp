% Part of the MetaUML manual.
% Copyright (C) 2005 Ovidiu Gheorghies
%
% This program is free software; you can redistribute it and/or
% modify it under the terms of the GNU General Public License
% as published by the Free Software Foundation; either version 2
% of the License, or (at your option) any later version.
%
% This program is distributed in the hope that it will be useful,
% but WITHOUT ANY WARRANTY; without even the implied warranty of
% MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
% GNU General Public License for more details.
%
% You should have received a copy of the GNU General Public License
% along with this program; if not, write to the Free Software
% Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.

input metauml;

beginfig(1);
  Class.Woonplaats("Woonplaats")
    ("+woonplaatsNaam",
     "+woonplaatsGeometrie")();
  Class.Nummeraanduiding("Nummeraanduiding") ()();
  Class.OpenbareRuimte("OpenbareRuimte") ()();
  Class.GerelateerdeAdressen("GerelateerdeAdressen") ()();
  Class.AdresseerbaarObject("_AdresseerbaarObject") ()();
  Class.Ligplaats("Ligplaats") ()();
  Class.Verblijfsobject("Verblijfsobject")
    ("+identificatie",
     "+verblijfsobjectGeometrie",
     "+gebruiksdoelVerblijfsobject",
     "+oppervlakteVerblijfsobject")();
  Class.Standplaats("Standplaats") ()();
  Class.Pand("Pand")
    ("+identificatie",
     "+pandGeometrie",
     "+pandstatus"
    )();

  Woonplaats.nw = (0,0);
  OpenbareRuimte.ne = Woonplaats.sw + (-40,5);
  Nummeraanduiding.n = Woonplaats.s + (0,-20);
  GerelateerdeAdressen.n = Nummeraanduiding.s + (0,-30);
  AdresseerbaarObject.e = GerelateerdeAdressen.w + (-100,0);
  Verblijfsobject.n = AdresseerbaarObject.s + (0,-40);
  Ligplaats.ne = Verblijfsobject.nw + (-20,0);
  Standplaats.nw = Verblijfsobject.ne + (20,0);
  Pand.n = Verblijfsobject.s + (0,-20);

  drawObject(Woonplaats);
  drawObject(Nummeraanduiding);
  drawObject(OpenbareRuimte);
  drawObject(GerelateerdeAdressen);
  drawObject(AdresseerbaarObject);
  drawObject(Verblijfsobject);
  drawObject(Ligplaats);
  drawObject(Standplaats);
  drawObject(Pand);

  link(association)(Woonplaats.s -- Nummeraanduiding.n);
  link(association)(Woonplaats.w -- OpenbareRuimte.n);
  link(association)(Nummeraanduiding.w -- OpenbareRuimte.s);
  link(association)(Nummeraanduiding.s + (30,0) -- GerelateerdeAdressen.n + (30,0));
  link(association)(Nummeraanduiding.s - (30,0) -- GerelateerdeAdressen.n - (30,0));
  link(composition)(GerelateerdeAdressen.w -- AdresseerbaarObject.e);
  link(inheritance)(Ligplaats.n -- AdresseerbaarObject.sw);
  link(inheritance)(Verblijfsobject.n -- AdresseerbaarObject.s);
  link(inheritance)(Standplaats.n -- AdresseerbaarObject.se);
  link(association)(Verblijfsobject.s -- Pand.n);

  item(iAssoc)("+gerelateerdPand")(obj.se = Pand.n);
  item(iAssoc)("1..*")(obj.sw = Pand.n);

  item(iAssoc)("+gerelateerdeAdressen")(obj.sw = AdresseerbaarObject.e+(0,5));
  item(iAssoc)("1")(obj.nw = AdresseerbaarObject.e+(0,-5));

  item(iAssoc)("+nevenadres")(obj.se = GerelateerdeAdressen.n - (30,0));
  item(iAssoc)("1..*")(obj.sw = GerelateerdeAdressen.n - (30,0));

  item(iAssoc)("+hoofdadres")(obj.se = GerelateerdeAdressen.n + (30,0));
  item(iAssoc)("1..*")(obj.sw = GerelateerdeAdressen.n + (30,0));

  item(iAssoc)("0..*")(obj.nw = Nummeraanduiding.s - (30,0));
  item(iAssoc)("1")(obj.nw = Nummeraanduiding.s + (30,0));

  item(iAssoc)("+gerelateerdeWoonplaats")(obj.ne = Woonplaats.s);
  item(iAssoc)("0..1")(obj.nw = Woonplaats.s);

  item(iAssoc)("+gerelateerdeWoonplaats")(obj.sw = Woonplaats.e);
  item(iAssoc)("1")(obj.nw = Woonplaats.e);
  item(iAssoc)("1..*")(obj.sw = OpenbareRuimte.n);

  item(iAssoc)("+gerelateerdeOpenbareRuimte") (obj.ne = OpenbareRuimte.s);
  item(iAssoc)("1")(obj.nw = OpenbareRuimte.s);
  item(iAssoc)("0..*")(obj.nw = Nummeraanduiding.e);

endfig;

end
