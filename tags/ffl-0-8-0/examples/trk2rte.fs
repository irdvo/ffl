\ ==============================================================================
\
\         trk2rte - gpx track to route convertor
\
\               Copyright (C) 2010  Dick van Oudheusden
\  
\ This library is free software; you can redistribute it and/or
\ modify it under the terms of the GNU General Public
\ License as published by the Free Software Foundation; either
\ version 2 of the License, or (at your option) any later version.
\
\ This library is distributed in the hope that it will be useful,
\ but WITHOUT ANY WARRANTY; without even the implied warranty of
\ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
\ General Public License for more details.
\
\ You should have received a copy of the GNU General Public
\ License along with this library; if not, write to the Free
\ Software Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
\
\ ==============================================================================
\ 
\  $Date: 2009-05-28 17:35:58 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/stc.fs
include ffl/snn.fs
include ffl/snl.fs
include ffl/sni.fs
include ffl/str.fs
include ffl/tos.fs
include ffl/xis.fs
include ffl/xos.fs

\ Data structure

begin-structure point%
  snn%
  +field      point>snn
  ffield:     point>lon
  ffield:     point>lat
end-structure

begin-structure route%
  snn%
  +field      route>snn
  snl%
  +field      route>points
  sni%
  +field      route>point-iter
  tos%
  +field      route>lon-tos
  tos%
  +field      route>lat-tos
  tos%
  +field      route>wpt-tos
  str%
  +field      route>prefix
  str%
  +field      route>name
  str%
  +field      route>description
  field:      route>segments
  ffield:     route>threshold
end-structure

begin-structure trk2rte%
  snl%
  +field      trk2rte>routes
  sni%
  +field      trk2rte>route-iter
  xis%
  +field      trk2rte>parser
end-structure


\ ------------------------------------------------------------------------------
\ Allocators
\ ------------------------------------------------------------------------------

: point-new  ( -- point = Allocate a new track point )
  point% allocate throw 
  >r
  r@ point>snn snn-init
  -512E+0 r@ point>lon f!
  -512E+0 r@ point>lat f!
  r>
;

: route-new  ( -- route = Allocate a new route )
  route% allocate throw
  >r
  r@ route>snn         snn-init
  r@ route>points  dup snl-init
  r@ route>point-iter  sni-init
  r@ route>lon-tos     tos-init
  r@ route>lat-tos     tos-init
  r@ route>wpt-tos     tos-init
  r@ route>description str-init
  r@ route>name        str-init
  r@ route>prefix      str-init
  r@ route>segments    0!
  0.0E+0
  r@ route>threshold   f!
  r>
;

: trk2rte-new  ( -- trk2rte = Allocate a new trk2rte variable )
  trk2rte% allocate throw
  >r
  r@ trk2rte>routes   dup snl-init
  r@ trk2rte>route-iter   sni-init
  r@ trk2rte>parser       xis-init
  true
  r@ trk2rte>parser       xis-strip!
  r>
;


\ ------------------------------------------------------------------------------
\ Deallocators
\ ------------------------------------------------------------------------------

: point-free  ( point -- = Free the point )
  ?free throw
;

: route-free  ( route -- = Free the route )
  >r 
  ['] point-free 
  r@ route>points      snl-(free)
  r@ route>lon-tos     tos-(free)
  r@ route>lat-tos     tos-(free)
  r@ route>wpt-tos     tos-(free)
  r@ route>prefix      str-(free)
  r@ route>name        str-(free)
  r@ route>description str-(free)
  r> free throw
;

: trk2rte-free  ( trk2rte -- = Free the trk2rte variable )
  >r
  ['] route-free
  r@ trk2rte>routes       snl-(free)
  r@ trk2rte>parser       xis-(free)
  r> free throw
;


\ ------------------------------------------------------------------------------
\ Distance and Bearing
\ ------------------------------------------------------------------------------

PI 180.0E+0 f/  fconstant Deg2Rad
180.0E+0 PI f/  fconstant Rad2Deg

6371000.0E+0    fconstant EarthRadius        \ in meters
   
[UNDEFINED] f2swap [IF]
: f2swap  ( r1 r2 r3 r4 -- r3 r4 r1 r2 )
  f>r f-rot fr> f-rot
;
[THEN]

: trk2rte-calc-distance  ( point1 point2 -- r = Calculate the haversine distance r in meters between point1 and point2 )
  over point>lat f@ Deg2Rad f* fcos          \ cos(from.lat)
  dup  point>lat f@ Deg2Rad f* fcos f*       \ a = cos(from.lat) * cos(to.lat)
  over point>lon f@ dup point>lon f@ f- Deg2Rad f* \ dLon = (from.lon - to.lon) 
  0.5E+0 f* fsin fdup f* f*                  \ b = a * sin(0.5 * dLon)
  swap point>lat f@     point>lat f@ f- Deg2Rad f* \ dLat = (from.lat - to.lat)
  0.5E+0 f* fsin fdup f* f+                  \ c= b + sin(0.5 * dLat)
  fsqrt fasin 2.0E+0 f* EarthRadius f*       \ distance = 2.0 * asin(sqrt(c) * EarthRadius
;

: trk2rte-calc-bearing  ( point1 point2 -- r = Calculate the bearing r in degrees between point1 and point2 )
  over point>lat f@ Deg2Rad f*
  dup  point>lat f@ Deg2Rad f*
  f2dup
  fsin fswap fcos f*                        \ lat1 lat2 a = sin(lat2)*cos(lat1)
  f-rot fswap fover                         \ a lat2 lat1 lat2 
  fcos fswap fsin f*                        \ a lat2 b = sin(lat1)*cos(lat2)
  point>lon f@ point>lon f@ f- Deg2Rad f*   \ a lat2 b dLon
  fswap fover fcos f*                       \ a lat2 dLon c = b*cos(dLon)
  f-rot fsin fswap fcos f*                  \ a c d = sin(dLin) * cos(lat2)
  f-rot f- fatan2 Rad2Deg f*                \ angle = atan2(d, a-c)
;

0 [IF]
point-new point-new
over dup 51.79163E+0 point>lat f!  4.68018E+0 point>lon f!
dup  dup 52.50953E+0 point>lat f!  6.09741E+0 point>lon f! 
2dup trk2rte-calc-distance ." Expected distance = 125.4km, calculated distance = " 1000E+0 f/ f. cr
2dup trk2rte-calc-bearing  ." Expected bearing  = 49.9,    calculated bearing  = " f. cr 
dup  dup 51.38207E+0 point>lat f! -2.36206E+0 point>lon f!
2dup trk2rte-calc-distance ." Expected distance = 488.5km, calculated distance = " 1000E+0 f/ f. cr
2dup trk2rte-calc-bearing  ." Expected bearing  = 267.5,   calculated bearing  = " 360.0E+0 f+ f. cr 
dup  dup -34.57895E+0 point>lat f! -58.42529E+0 point>lon f!
2dup trk2rte-calc-distance ." Expected distance = 11390km, calculated distance = " 1000E+0 f/ f. cr
2dup trk2rte-calc-bearing  ." Expected bearing  = 228.7,   calculated bearing  = " 360.0E+0 f+ f. cr 

[THEN]


\ ------------------------------------------------------------------------------
\ Reducer
\ ------------------------------------------------------------------------------

: trk2rte-reduce  ( route -- = Reduce the route with the angle threshold )
  >r
  r@ route>points snl-length@ 2 > IF
    r@ route>point-iter sni-first           \ prev-2
    r@ route>point-iter sni-next            \ prev
    2dup trk2rte-calc-bearing               \ last-bearing
    BEGIN
      r@ route>point-iter sni-next          \ current
      r@ route>point-iter sni-last? 0=      \ Reduce till last node
    WHILE
      2dup trk2rte-calc-bearing             \ bearing
      f2dup f- fabs r@ route>threshold f@ f< IF  \ S: prev-2 prev current F: last-bearing bearing R: trk2rte route
        nip                                 \ skip point -> drop prev and bearing
        over r@ route>points snl-remove-after point-free
        fdrop
      ELSE
        rot drop                            \ keep point -> drop prev-2 and last bearing
        fswap fdrop
      THEN
    REPEAT
    drop 2drop
    fdrop
  THEN
  rdrop
;

\ ------------------------------------------------------------------------------
\ GPX Reader
\ ------------------------------------------------------------------------------

: trk2rte-add-distance  ( r1 prev-point point -- r2 prev-point = Add the distance to r1 between prev-point and point )
  over nil<> IF
    2dup trk2rte-calc-distance f+
  THEN
  nip
;


: trk2rte-reader  ( fileid -- c-addr u | 0 = Read data from a file )
  pad 64 rot read-file throw
  dup IF
    pad swap
  THEN
;


: trk2rte-route-add  ( trk2rte -- route = Add a route )
  cr ." == Route segment " dup trk2rte>routes snl-length@ 1+ . ." == "
  
  route-new >r 
  
  cr ." Enter the waypoint prefix (3 chars)                 : "
  pad 3 accept ?dup IF
    pad swap 
  ELSE
    s" WPT"
  THEN
  r@ route>prefix str-set
    
  cr ." Enter the (short) route name                        : "
  pad 80 accept ?dup IF
    pad swap r@ route>name str-set
  THEN

  cr ." Enter the route description                         : "
  pad 80 accept ?dup IF
    pad swap r@ route>description str-set
  THEN
    
  
  r@ swap trk2rte>routes snl-append
  r>
;


: trk2rte-route-close  ( route -- = Close the route )
  >r
  cr ." Reduce the route                              (y/n) : "
  pad 2 accept IF
    pad c@ chr-upper [char] Y = IF
      BEGIN
        cr ." Angle threshold during reducing           (degrees) : "
        pad 20 accept ?dup IF
          pad swap >float dup IF
            r@ route>threshold f!
            r@ trk2rte-reduce  
          THEN
        ELSE
          false
        THEN
      UNTIL
    THEN
  THEN

  cr ." -> " r@ route>points snl-length@ . ." track point(s); distance: " 0.0E+0 nil ['] trk2rte-add-distance r@ route>points snl-execute drop fround f. ." meter."
  rdrop
;


: trk2rte-add-point ( i*x n c-addr u route -- c-addr u 0 = Add a point with lat,lon to the current route )
  >r
  r@ nil<> IF
    point-new dup r@ route>points snl-append \ Allocate new point
    -rot 2>r                                \ S: file xis i*x n point
    swap 0 DO
      >r
      2swap
      2dup s" lat" icompare 0= IF
        2drop >float IF r@ point>lat f! THEN
      ELSE s" lon" icompare 0= IF
        >float IF r@ point>lon f! THEN
      ELSE
        2drop
      THEN THEN
      r>
    LOOP
    drop                                    \ point
    0 2r>                                   \ restore stack for remove-read-parameters
  ELSE
    cr ." Warning: Track point without track segment, skipping.." cr
  THEN
  rdrop
;


: trk2rte-read  ( trk2rte -- = Read the gpx input files )
  >r
  BEGIN
    cr ." Enter the gpx input filename           (Enter=Done) : "
    pad 80 accept
    ?dup
  WHILE
    pad swap r/o open-file ?dup 0= IF
      dup ['] trk2rte-reader r@ trk2rte>parser xis-set-reader
      
      r@ trk2rte>parser                     \ S: file xis
      nil >r
      BEGIN
        dup xis-read dup xis.error <> over xis.done <> AND
      WHILE
        >r                                  \ R: trk2rte route tag
        r@ xis.start-tag = IF
          2dup s" trkseg" icompare 0= IF
            r> rdrop r@ trk2rte-route-add >r >r  \ switch to new route
          ELSE 2dup s" trkpt" icompare 0= IF
            r'@ trk2rte-add-point           \ add track point
          THEN THEN
        ELSE r@ xis.end-tag = IF
          2dup s" trkseg" icompare 0= IF
            r'@ trk2rte-route-close
            r> rdrop nil >r >r              \ switch to no route
          THEN
        ELSE r@ xis.empty-element = IF
          2dup s" trkpt" icompare 0= IF
            r'@ trk2rte-add-point           \ add track point
          THEN
        THEN THEN THEN
        r>
        xis+remove-read-parameters
      REPEAT
      
      xis.error = IF
        cr ."  -> Error in file."           \ XXX: stop?
      THEN
      drop                                  \ trk2rte>parser
      close-file throw
      rdrop                                 \ route
    ELSE
      drop                                  \ fileid
      ."  -> File not found."
    THEN
    cr
  REPEAT
  rdrop
;


\ ------------------------------------------------------------------------------
\ GPX Writer
\ ------------------------------------------------------------------------------

: trk2rte-writer  ( c-addr u fileid -- flag = Write text c-addr u to file )
  write-file 0=
;


: trk2rte-gpx-wpt-point  ( n tos route point -- n tos route = Callback: write the point to the gpx file, with waypoint counter n )
  dup point>lat f@  point>lon f@

  2>r
  s" lon"                               \ Format lon
  r@ route>lon-tos >r
  r@ tos-rewrite
  r@ tos-write-fixed-point
  r> str-get
  
  s" lat"                               \ Format lat
  r@ route>lat-tos >r
  r@ tos-rewrite
  r@ tos-write-fixed-point
  r> str-get
  2 s" wpt" r'@ xos-write-start-tag     \ Write the wpt start tag
  
  0 s" name" r'@ xos-write-start-tag
  
  r@ route>wpt-tos >r                   \ Format waypoint name and ..
  r@ tos-rewrite
  r'@ route>prefix str-get r@ tos-write-string
  dup r@ tos-write-number
  [char] 0 3 r@ tos-align-right
  r> str-get r'@ xos-write-text         \ .. write in gpx file
  
  s" name" r'@ xos-write-end-tag
  s" wpt" r'@ xos-write-end-tag
  r'@ tos-write-line
  r'@ tos-flush

  1+                                    \ Increment waypoint number
  2r>
;


: trk2rte-gpx-wpt-route  ( tos route -- tos = Callback: write the route to gpx-wpt file )
  1 -rot ['] trk2rte-gpx-wpt-point over route>points snl-execute drop nip
;


: trk2rte-gpx-rte-point  ( n tos route point -- n tos route = Callback: write the point to the gpx-rte file )
  dup point>lat f@  point>lon f@

  2>r
  s" lon"                               \ Format lon
  r@ route>lon-tos >r
  r@ tos-rewrite
  r@ tos-write-fixed-point
  r> str-get
  
  s" lat"                               \ Format lat
  r@ route>lat-tos >r
  r@ tos-rewrite
  r@ tos-write-fixed-point
  r> str-get
  2 s" rtept" r'@ xos-write-start-tag   \ Write the rtept start tag
  
  0 s" name" r'@ xos-write-start-tag
  
  r@ route>wpt-tos >r                   \ Format point name and ..
  r@ tos-rewrite
  r'@ route>prefix str-get r@ tos-write-string
  dup r@ tos-write-number
  [char] 0 3 r@ tos-align-right
  r> str-get r'@ xos-write-text         \ .. write in gpx file
  
  s" name" r'@ xos-write-end-tag
  s" rtept" r'@ xos-write-end-tag
  r'@ tos-write-line
  r'@ tos-flush

  1+                                    \ Increment waypoint number
  2r>
;


: trk2rte-gpx-rte-route  ( tos route -- tos = Callback: write the route to the gpx-rte file )
  2>r
  
  0 s" rte" r'@ xos-write-start-tag
  r'@ tos-write-line
      
  r@ route>name str-empty? 0= IF
    0 s" name" r'@ xos-write-start-tag
    r@ route>name str-get r'@ xos-write-text
    s" name" r'@ xos-write-end-tag
    r'@ tos-write-line
  THEN
      
  r@ route>description str-empty? 0= IF
    0 s" desc" r'@ xos-write-start-tag
    r@ route>description str-get r'@ xos-write-text
    s" desc" r'@ xos-write-end-tag
    r'@ tos-write-line
  THEN

  1 2r@ ['] trk2rte-gpx-rte-point over route>points snl-execute 2drop drop
  
  s" rte" r'@ xos-write-end-tag
  r'@ tos-write-line
  r'@ tos-flush
  2r> drop
;


: trk2rte-write-gpx  ( trk2rte -- = Write the gpx output file )
  >r
  cr ." Enter the gpx output filename          (Enter=None) : "
  pad 80 accept ?dup IF
    pad swap w/o create-file 0= IF
      tos-new >r
      dup ['] trk2rte-writer r@ tos-set-writer
    
      s" version" s" 1.0" 1 r@ xos-write-start-xml
      r@ tos-write-line
      s" creator" s" trk2rte" s" version" s" 1.1" 2 s" gpx" r@ xos-write-start-tag
      r@ tos-write-line

      r@ ['] trk2rte-gpx-wpt-route r'@ trk2rte>routes snl-execute drop

      r@ ['] trk2rte-gpx-rte-route r'@ trk2rte>routes snl-execute drop
    
      s" gpx" r@ xos-write-end-tag
      r@ tos-write-line

      r@ tos-flush
      r> tos-free
      close-file throw
      ."  -> Done."
    ELSE
      drop                                  \ fileid
      ."  -> Unable to create file."
    THEN
  THEN
  rdrop
;

\ ------------------------------------------------------------------------------
\ OZI Writer
\ ------------------------------------------------------------------------------

: trk2rte-ozi-wpt-point  ( n1 n2 tos route point -- n1 n2 tos route = Callback: write every point to ozi waypoint file, with total counter n1 and waypoint number n2 )
  dup point>lon f@  point>lat f@
  2>r
  \ 1,AAA1,51.780559,4.552824,25569.00000,0,1,3,0,65535,AAA1,0,0,0,-777,6,0,17
  over                           r'@ tos-write-number
  [char] ,                       r'@ tos-write-char
  r@ route>prefix str-get        r'@ tos-write-string
  dup                            r'@ tos-write-number
  [char] 0 3                     r'@ tos-align-right
  [char] ,                       r'@ tos-write-char
                                 r'@ tos-write-fixed-point
  [char] ,                       r'@ tos-write-char
                                 r'@ tos-write-fixed-point
  [char] ,                       r'@ tos-write-char
  s" 25569.00000,0,1,3,0,65535," r'@ tos-write-string
  r@ route>prefix str-get        r'@ tos-write-string
  dup                            r'@ tos-write-number
  [char] 0 3                     r'@ tos-align-right
  s" ,0,0,0,-777,6,0,17"         r'@ tos-write-string
                                 r'@ tos-write-line
                                 r'@ tos-flush
  swap 1+ swap 1+
  2r>
;


: trk2rte-ozi-wpt-route  ( n tos route -- n tos = Callback: write every route to ozi waypoint file, n is total counter )
  1 -rot                                    \ Waypoint number
  ['] trk2rte-ozi-wpt-point over route>points snl-execute
  drop nip                                  \ Route, waypoint number
;


: trk2rte-write-ozi-wpt  ( trk2rte -- = Write the ozi wpt output file )
  >r
  cr ." Enter the ozi waypoint output filename (Enter=None) : "
  pad 80 accept ?dup IF
    pad swap w/o create-file 0= IF
      tos-new >r
      dup ['] trk2rte-writer r@ tos-set-writer

      s" OziExplorer Waypoint File Version 1.1" r@ tos-write-string r@ tos-write-line
      s" WGS 84"                                r@ tos-write-string r@ tos-write-line
      s" Reserved 2"                            r@ tos-write-string r@ tos-write-line
      s" Reserved 3"                            r@ tos-write-string r@ tos-write-line
    
      1 r@ ['] trk2rte-ozi-wpt-route r'@ trk2rte>routes snl-execute 2drop
    
      r@ tos-flush
      r> tos-free
      close-file throw
      ."  -> Done."
    ELSE
      ."  -> Unable to create file."
    THEN
  THEN
  rdrop
;


: trk2rte-ozi-rte-point  ( n1 n2 n3 tos route point -- n1 n2 n3 tos route = Callback: write every point to ozi route file, with route number n1, total counter n2 and waypoint number n3 )
  dup point>lon f@  point>lat f@
  2>r
  \ W,2,,70,BBB1,51.415267,4.345651,25569.00000,0,1,3,0,65535,,0,0
  s" W,"                              r'@ tos-write-string
  rot dup                             r'@ tos-write-number -rot 
  [char] ,                            r'@ tos-write-char
  [char] ,                            r'@ tos-write-char
  over                                r'@ tos-write-number
  [char] ,                            r'@ tos-write-char
  r@ route>prefix str-get             r'@ tos-write-string
  dup                                 r'@ tos-write-number
  [char] 0 3                          r'@ tos-align-right
  [char] ,                            r'@ tos-write-char
                                      r'@ tos-write-fixed-point
  [char] ,                            r'@ tos-write-char
                                      r'@ tos-write-fixed-point
  s" ,25569.00000,0,1,3,0,65535,,0,0" r'@ tos-write-string
                                      r'@ tos-write-line
                                      r'@ tos-flush
  swap 1+ swap 1+
  2r>

;


: trk2rte-ozi-rte-route  ( n1 n2 tos route -- n1 n2 tos = Callback: write every route to ozi route file, route counter n1 and total counter n2 )
  2>r
  \ R,2,TRUG,,
  s" R,"                       r'@ tos-write-string
  over                         r'@ tos-write-number
  [char] ,                     r'@ tos-write-char
  r@ route>name str-get        r'@ tos-write-string
  [char] ,                     r'@ tos-write-char
  r@ route>description str-get r'@ tos-write-string
  [char] ,                     r'@ tos-write-char
                               r'@ tos-write-line
                               r'@ tos-flush
  1 2r@ ['] trk2rte-ozi-rte-point over route>points snl-execute 2drop drop
  swap 1+ swap
  2r> drop
;


: trk2rte-write-ozi-rte  ( trk2rte -- = Write the ozi rte output file )
  >r
  cr ." Enter the ozi route output filename    (Enter=None) : "
  pad 80 accept ?dup IF
    pad swap w/o create-file 0= IF
      tos-new >r
      dup ['] trk2rte-writer r@ tos-set-writer

      s" OziExplorer Route File Version 1.0" r@ tos-write-string r@ tos-write-line
      s" WGS 84"                             r@ tos-write-string r@ tos-write-line
      s" Reserved 1"                         r@ tos-write-string r@ tos-write-line
      s" Reserved 2"                         r@ tos-write-string r@ tos-write-line

      1 1 r@ ['] trk2rte-ozi-rte-route r'@ trk2rte>routes snl-execute 2drop drop

      r@ tos-flush
      r> tos-free
      close-file throw
      ."  -> Done."
    ELSE
      ."  -> Unable to create file."
    THEN
  THEN
  rdrop
;


: trk2rte-write  ( trk2rte -- = Write the gpx or ozi output files )
  >r
  r@ trk2rte-write-gpx
  r@ trk2rte-write-ozi-wpt
  r> trk2rte-write-ozi-rte
;


\ ------------------------------------------------------------------------------
\ Main program
\ ------------------------------------------------------------------------------

: trk2rte  ( -- = Main program )
  precision 
  10 set-precision
  trk2rte-new >r
  r@ trk2rte-read
  r@ trk2rte-write
  r> trk2rte-free
  set-precision
;


trk2rte cr

bye
