\ ==============================================================================
\
\           hnt_expl - the base hash table example in the ffl
\
\               Copyright (C) 2007  Dick van Oudheusden
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
\  $Date: 2007-12-09 07:23:14 $ $Revision: 1.4 $
\
\ ==============================================================================

include ffl/hnt.fs
include ffl/hni.fs


\ Example: store scientific constants in a hash table

\ Extend the base node with a float for storing the value of the constant

begin-structure sc%
  hnn%
  +field  sc>ht                      \ Node extends the base node
  ffield: sc>value                   \ Value of constant
end-structure


\ Create the hash table in the dictionary with an initial size of 10

10 hnt-create sc-table


\ Word for adding the value

: sc-add ( r c-addr u -- = value key )
  2dup sc-table hnt-search                     \ Search if the key is already present in the hash table
  dup nil<> IF                                 \ If the key is already present Then
    nip nip nip                                \   Remove the key and hash value from stack
    sc>value f!                                \   Update the node in the hash table with the value
  ELSE                                         \ Else
    drop                                       \   Drop nil
    sc% allocate throw                         \   Allocate a new sc% node
    >r
    r@ hnn-init                                \   Initialise the new sc% node with the key and hash
    r@ sc>value f!                             \   Save the value in the node
    r> sc-table hnt-insert                     \   Store the sc% node in the hash table
  THEN
;


\ Add the constants

3.14158E+0 s" pi"     sc-add
2.71828E+0 s" euler"  sc-add
1.61803E+0 s" golden" sc-add

\ Word for printing the scientific constant

: sc-emit ( sc% -- )
  dup hnn-key@ type ."  -> " sc>value f@ f. cr
;


\ Print all scientific constants

' sc-emit sc-table hnt-execute                \ Execute the word sc-emit for all entries in the hash table


\ Example hash table iterator

\ Create the hash table iterator in the dictionary

sc-table hni-create sc-iter                   \ Create an iterator named sc-iter on the sc-table hash table

\ Moving the iterator

sc-iter hni-first                             \ Move the iterator to the first record
nil<> [IF]                                    \ If record exists Then ..
  sc-iter hni-key type                        \   Type the key ..
  .(  => )
  sc-iter hni-get sc>value f@ f.              \   .. and the value
  cr
[THEN]

sc-iter hni-next                              \ Move the iterator to the next record
nil<> [IF]                                    \ If record exists Then ..
  sc-iter hni-key type                        \   Type the key ..
  .(  => )
  sc-iter hni-get sc>value f@ f.              \   .. and the value
  cr
[ELSE]
  .( No next record in the hash table) cr  
[THEN]


