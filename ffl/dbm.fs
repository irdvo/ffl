\ ==============================================================================
\
\              dbm - the gdbm interface module in the ffl
\
\               Copyright (C) 2009  Dick van Oudheusden
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


include ffl/config.fs


[UNDEFINED] dbm.version [IF]

1 chars 1 =  cell 3 > AND [IF]

include ffl/stc.fs
include ffl/str.fs


( dbm = gdbm interface )
( The dbm module implements an interface to gnu's dbm. It implements words to )
( open, create, fetch, write, reorganise and close a gdbm file. See           )
( [http://www.gnu.org/software/gdbm/ gdbm's home] for more info.              )


1 constant dbm.version

( private avail-element structure )

begin-structure dbm%ave%
  field:  dbm>ave>av-size         \ the size of the avaible block
  field:  dbm>ave>av-addr         \ the file address of the available block
end-structure


( private avail-block structure )

begin-structure dbm%avb%
  field:  dbm>avb>size            \ the number of avail elements in the table
  field:  dbm>avb>count           \ the number of entries in the table
  field:  dbm>avb>next-block      \ the address of the next avail-block
  dbm%ave%
  +field  dbm>avb>av-table        \ the avail-table
end-structure


( private header structure )

begin-structure dbm%hdr%
  field:  dbm>hdr>header-magic    \ the header code
  field:  dbm>hdr>block-size      \ the block size
  field:  dbm>hdr>dir             \ the file address of the hash directory table
  field:  dbm>hdr>dir-size        \ the hash directory table size
  field:  dbm>hdr>dir-bits        \ the number of address bits used in the table
  field:  dbm>hdr>bucket-size     \ the size of a hash bucket
  field:  dbm>hdr>bucket-elems    \ the number of elements in a hash bucket
  field:  dbm>hdr>next-block      \ the next unallocated block address
  dbm%avb%
  +field  dbm>hdr>avail           \ the avail-block, it grows to fill the entire block
end-structure


( dbm structure )

begin-structure dbm%       ( -- n = Get the required space for a dbm variable )
  str%
  +field  dbm>name           \ the file name
  field:  dbm>read-write     \ the read\write status
  field:  dbm>fast-write     \ the fast write state
  field:  dbm>central-free   \ if set all free blocks are kept in the header
  field:  dbm>coalesce-blocks \ if set free blocks are tried to be merged
  field:  dbm>file-locking   \ should file locking be performed
  field:  dbm>file           \ the file
  field:  dbm>header         \ the file header
  field:  dbm>dir            \ the hash table directory
  field:  dbm>bucket-cache   \ the bucket cache
  field:  dbm>cache-size     \ the size of the bucket cache
  field:  dbm>last-read      \ the last read
  field:  dbm>bucket         \ the current hash bucket in the cache
  field:  dbm>bucket-dir     \ the directory entry for the current hash bucket
  field:  dbm>cache-entry    \ the current bucket cache entry
  field:  dbm>header-changed \ the indication that the header is changed
  field:  dbm>directory-changed \ the indication that the directory is changed
  field:  dbm>bucket-changed \ the indication that the bucket is changed
  field:  dbm>second-changed \ the indication that the second is changed
end-structure


( gzip file reader variable creation, initialisation and destruction )

: dbm-init         ( dbm -- = Initialise the dbm variable )
  >r
  r@ dbm>name              str-init
  r@ dbm>read-write        off
  r@ dbm>fast-write        off
  r@ dbm>central-free      off
  r@ dbm>coalesce-blocks   off
  r@ dbm>file-locking      off
  r@ dbm>file              0!
  r@ dbm>header            nil!
  r@ dbm>dir               nil!
  r@ dbm>bucket-cache      nil!
  r@ dbm>cache-size        0!
  r@ dbm>last-read         0!
  r@ dbm>bucket            nil!
  r@ dbm>bucket-dir        0!
  r@ dbm>cache-entry       nil!
  r@ dbm>header-changed    off
  r@ dbm>directory-changed off
  r@ dbm>bucket-changed    off
  r> dbm>second-changed    off
;


: dbm-(free)       ( dbm -- = Free the private dynamic variables from the heap )
  dup dbm>name str-(free)
  \ r@ dbm>header       @ ?free throw
  \ r@ dbm>dir          @ ?free throw
  \ r@ dbm>bucket-cache @ ?free throw
  \ r@ dbm>bucket       @ ?free throw
  \ r@ dbm>cache-entry  @ ?free throw
  drop
;


: dbm-create       ( "<spaces>name" -- ; -- dbm = Create a dbm variable in the dictionary )
  create  here  dbm% allot  dbm-init
;


: dbm-new          ( n -- car = Allocate a dbm variable on the heap )
  dbm% allocate throw  dup dbm-init 
;


: dbm-free         ( car -- = Free the dbm variable from the heap )
  dup dbm-(free)
   
  free throw
;


( Module words )

: dbm+hash         ( c-addr u -- +n = Calculate the hash used in the gdbm files )
  [ hex ] 238F13AF [ decimal ]
  over *                         \ value = 238F13AF * length
  swap 0 ?DO
    over I chars + c@
    I 5 * 24 mod
    lshift +                     \ value = (value + (ch << index * 5 % 24)) & 7F..F
    [ hex ] 7FFFFFFF [ decimal ] AND
  LOOP
  nip
  1103515243 * 12345 +           \ value = (value * 1103515243 + 12345) & 7F..F
  [ hex ] 7FFFFFFF [ decimal ] AND
;


( Private file words )

( Dbm open constants )

1 constant dbm.reader   ( -- n = Open flag for only reading a gdbm file )
2 constant dbm.writer   ( -- n = Open flag for reading and writing on an existing gdbm file )
3 constant dbm.wrcreat  ( -- n = Open flag for reading and writing on a gdbm file, if it doesn't exist, it is created )
4 constant dbm.newdb    ( -- n = Open flag for reading and writing on a gdbm file, which is always created )
8 constant dbm.sync     ( -- n = Extra open flag for writers: flush the data to the gdbm file after a change )

( Dbm store constants )

1 constant dbm.insert   ( -- n = Store flag for only inserting data, error if key is already present )
2 constant dbm.replace  ( -- n = Store falg for replace data, also if key is already present )


( File words )

: dbm-open         ( c-addr u +n1 n2 dbm -- ior = Open or create a gdbm file with name c-addr u, block size n1 and flags n2 )
;


: dbm-fetch        ( c-addr u dbm -- c-addr u ior = Fetch data from the gdbm file with key c-addr u )
;


: dbm-exist        ( c-addr u dbm -- flag ior = Check if the key c-addr u exists in the gdbm file )
;


: dbm-store        ( c-addr1 u1 c-addr2 u2 n dbm -- ior = Store data in the gdbm file with key c-addr2 u2, data c-addr1 u1 and flags n )
;


: dbm-delete      ( c-addr u dbm -- flag ior = Delete the key c-addr u from the gdbm file )
;


: dbm-first-key    ( dmb -- c-addr u ior = Get the first key from the gdbm file )
;


: dbm-next-key     ( dbm -- c-addr u ior = Get the next key from the gdbm file )
;


: dbm-reorganize   ( dbm -- ior = Reorganize the gdbm file )
;


: dbm-sync         ( dbm -- ior = Flush unwritten data to disk )
;


: dbm-close        ( dbm -- ior = Close the gdbm file )
;


( Inspection )

: dbm-dump         ( car -- = Dump the dbm variable )
  ." dbm:" dup . cr
    ." name             :" dup dbm>name str-dump
    ." read-write       :" dup dbm>read-write ? cr
    ." fast-write       :" dup dbm>fast-write ? cr
    ." central-free     :" dup dbm>central-free ? cr
    ." coalesce-blocks  :" dup dbm>coalesce-blocks ? cr
    ." file-locking     :" dup dbm>file-locking ? cr
    ." file             :" dup dbm>file ? cr
  \ ." header           :" dup dbm>header @ dbm-hdr-dump
  \ ." dir              :" dup dbm>dir ? cr
  \ ." bucket-cache     :" dup dbm>bucket-cache ? cr
    ." cache-size       :" dup dbm>cache-size ? cr
    ." last-read        :" dup dbm>last-read ? cr
  \ ." bucket           :" dup dbm>bucket ? cr
    ." bucket-dir       :" dup dbm>bucket-dir ? cr
  \ ." cache-entry      :" dup dbm>cache-entry ? cr
    ." header-changed   :" dup dbm>header-changed ? cr
    ." directory-changed:" dup dbm>directory-changed ? cr
    ." bucket-changed   :" dup dbm>bucket-changed ? cr
    ." second-changed   :" dbm>second-changed ? cr
;

[ELSE]
.( Warning: dbm requires 1 byte chars and at least 4 byte cells ) cr
[THEN]

[THEN]

\ ==============================================================================
