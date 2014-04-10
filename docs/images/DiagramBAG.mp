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
  OpenbareRuimte.nw = Woonplaats.se + (30,5);
  Nummeraanduiding.n = Woonplaats.s + (0,-20);
  GerelateerdeAdressen.n = Nummeraanduiding.s + (0,-20);
  AdresseerbaarObject.e = GerelateerdeAdressen.w + (-20,0);
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
  link(association)(Woonplaats.e -- OpenbareRuimte.n);
  link(association)(Nummeraanduiding.e -- OpenbareRuimte.s);
  link(association)(Nummeraanduiding.s -- GerelateerdeAdressen.n);
  link(composition)(GerelateerdeAdressen.w -- AdresseerbaarObject.e);
  link(inheritance)(Ligplaats.n -- AdresseerbaarObject.s);
  link(inheritance)(Verblijfsobject.n -- AdresseerbaarObject.s);
  link(inheritance)(Standplaats.n -- AdresseerbaarObject.s);
  link(association)(Verblijfsobject.s -- Pand.n);


endfig;

end
