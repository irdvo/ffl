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
\  $Date: 2007-12-24 08:06:28 $ $Revision: 1.6 $
\
\ ==============================================================================

include ffl/xis.fs
include ffl/est.fs
include ffl/tst.fs


.( Testing: xis) cr

t{ xis-create xis1 }t


\ Single Tag tests

t{ s" <tag>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag ?s s" tag" compare ?0 ?0 }t

t{ s" hello<tag>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" compare ?0 }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 ?0 }t
t{ xis1 xis-read xis.text          ?s s" bye" compare ?0 }t

\ Tag with attribute without value test

t{ s" <tag attr1>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 ?0 ?nil }t

t{ s" hello<tag attr1>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" compare ?0 }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 ?0 ?nil }t
t{ xis1 xis-read xis.text          ?s s" bye" compare ?0 }t


\ Tag with attribute with non-quoted value

t{ s" <tag attr1=value1>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value1" compare ?0 }t


t{ s" <tag attr1 = value1>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value1" compare ?0 }t


t{ s" <tag attr1=value1 >" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value1" compare ?0 }t


t{ s" <tag attr1=&lt;1&amp;2&gt;>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" <1&2>" compare ?0 }t


t{ s" hello<tag attr1=value1>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" compare ?0 }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value1" compare ?0 }t
t{ xis1 xis-read xis.text          ?s s" bye" compare ?0 }t


\ Tag with attribute with single-quoted value

t{ s" <tag attr1='value 1'>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value 1" compare ?0 }t

t{ s" <tag attr1 = 'value 1'>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value 1" compare ?0 }t

t{ s" <tag attr1='value 1' >" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value 1" compare ?0 }t

t{ s" hello<tag attr1='value&amp;1'>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" compare ?0 }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value&1" compare ?0 }t
t{ xis1 xis-read xis.text          ?s s" bye" compare ?0 }t


\ Tag with attribute with double-quoted value

t{ s\" <tag attr1=\"value 1\">" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value 1" compare ?0 }t

t{ s\" <tag attr1 = \"value 1\">" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value 1" compare ?0 }t

t{ s\" <tag attr1=\"value 1\" >" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s" value 1" compare ?0 }t

t{ s\" hello<tag attr1=\"value&quot;1\">bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" compare ?0 }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" compare ?0 1 ?s s" attr1" compare ?0 s\" value\"1" compare ?0 }t
t{ xis1 xis-read xis.text          ?s s" bye" compare ?0 }t

\ Combination tag test 

t{ s\" <airplane brand=\"airtrain\" color=brown&amp;yellow type='three wings'/>" xis1 xis-set-string }t

t{ xis1 xis-read xis.empty-element ?s s" airplane" ?str 3 ?s s" type" ?str s" three wings" ?str s" color" ?str s" brown&yellow" ?str s" brand" ?str s" airtrain" ?str }t

\ Other tests

t{ s" <!--Comment1--><![CDATA[Hallo]]>&gt;Test1&amp;Test2&unknown;Test3&lt;<!--Comment2--></tag ><bold />" xis1 xis-set-string }t 

t{ xis1 xis-read xis.comment       ?s s" Comment1" compare ?0 }t
t{ xis1 xis-read xis.cdata         ?s s" Hallo"    compare ?0 }t
t{ xis1 xis-read xis.text          ?s s" >Test1&Test2&unknown;Test3<" compare ?0 }t
t{ xis1 xis-read xis.comment       ?s s" Comment2" compare ?0 }t
t{ xis1 xis-read xis.end-tag       ?s s" tag"      compare ?0 }t
\ t{ xis1 xis-read xis.empty-element ?s s" bold"     compare ?0 }t


\ ==============================================================================
