\ ==============================================================================
\
\        xos_test - the test words for the xos module in the ffl
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
\  $Date: 2008-03-02 15:03:03 $ $Revision: 1.7 $
\
\ ==============================================================================

include ffl/xos.fs
include ffl/est.fs
include ffl/tst.fs


.( Testing: xos) cr

t{ tos-create tos5 }t


: xos-test-write-start-xml
  s" standalone" s" yes" s" encoding" s" ISO-8559-1" s" version" s" 1.0" 3 
;

: xos-test-write-proc-instr
  s" mode" s" on" 1 s" pi"
;

: xos-test-write-start-tag
  s" code" s" <'>" 1 s" mail"
;

t{ xos-test-write-start-xml         tos5 xos-write-start-xml }t
t{ s" comment"                      tos5 xos-write-comment }t
t{ xos-test-write-proc-instr        tos5 xos-write-proc-instr }t
t{ xos-test-write-start-tag         tos5 xos-write-start-tag }t
t{ 0 s" to"                         tos5 xos-write-start-tag }t
t{ s" Bill & Sara"                  tos5 xos-write-text }t
t{ s" to"                           tos5 xos-write-end-tag }t
t{ 0 s" from"                       tos5 xos-write-start-tag }t
t{ s" $#70;orth"                    tos5 xos-write-raw-text }t
t{ s" from"                         tos5 xos-write-end-tag }t
t{ 0 s" contents"                   tos5 xos-write-start-tag }t
t{ s" : 2dup dup dup ;"             tos5 xos-write-cdata }t
t{ s" contents"                     tos5 xos-write-end-tag }t
t{ 0 s" others"                     tos5 xos-write-empty-element }t
t{ s" mail"                         tos5 xos-write-end-tag }t

t{ tos5 str-get
   s\" <?xml version=\"1.0\" encoding=\"ISO-8559-1\" standalone=\"yes\"?><!--comment--><?pi mode=\"on\"?><mail code=\"&lt;&apos;&gt;\"><to>Bill &amp; Sara</to><from>$#70;orth</from><contents><![CDATA[: 2dup dup dup ;]]></contents><others/></mail>"
   ?str }t


\ test DTDs

: xos-test-write-system-dtd
  s" order.dtd" nil 0 s" order"
;

: xos-test-write-internal-dtd
  s" <!ELEMENT mail (to+,from,contents,others+)>" s" mail"
;

: xos-test-write-public-dtd
  s" something" s" -//W3C//DTD HTML 4.0 Transitional//EN" nil 0 s" HTML"
;

t{ tos5 tos-rewrite }t

t{ xos-test-write-system-dtd  tos5 xos-write-system-dtd }t

t{ tos5 str-get s\" <!DOCTYPE order SYSTEM \"order.dtd\">" ?str }t


t{ tos5 tos-rewrite }t

t{ xos-test-write-internal-dtd tos5 xos-write-internal-dtd }t

t{ tos5 str-get s" <!DOCTYPE mail [<!ELEMENT mail (to+,from,contents,others+)>]>" ?str }t


t{ tos5 tos-rewrite }t

t{ xos-test-write-public-dtd tos5 xos-write-public-dtd }t

t{ tos5 str-get s\" <!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\" \"something\">" ?str }t


 
\ ==============================================================================
