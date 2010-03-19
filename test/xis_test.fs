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
\  $Date: 2008-01-13 08:09:33 $ $Revision: 1.11 $
\
\ ==============================================================================

include ffl/xis.fs
include ffl/est.fs
include ffl/tst.fs


.( Testing: xis) cr

t{ xis-create xis1 }t


\ Single Tag tests

t{ s" <tag>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag ?s s" tag" ?str ?0 }t

t{ s" hello<tag>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" ?str }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str ?0 }t
t{ xis1 xis-read xis.text          ?s s" bye" ?str }t


\ Tag with attribute without value test

t{ s" <tag attr1>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s ?0 ?nil s" attr1" ?str }t

t{ s" hello<tag attr1>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" ?str }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s ?0 ?nil s" attr1" ?str }t
t{ xis1 xis-read xis.text          ?s s" bye" ?str }t


\ Tag with attribute with non-quoted value

t{ s" <tag attr1=value1>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value1" ?str s" attr1" ?str }t


t{ s" <tag attr1 = value1>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value1" ?str s" attr1" ?str }t


t{ s" <tag attr1=value1 >" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value1" ?str s" attr1" ?str }t


t{ s" <tag attr1=&lt;1&amp;2&gt;>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" <1&2>" ?str s" attr1" ?str }t


t{ s" hello<tag attr1=value1>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" ?str }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value1" ?str s" attr1" ?str }t
t{ xis1 xis-read xis.text          ?s s" bye" ?str }t


\ Tag with attribute with single-quoted value

t{ s" <tag attr1='value 1'>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value 1" ?str s" attr1" ?str }t

t{ s" <tag attr1 = 'value 1'>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value 1" ?str s" attr1" ?str }t

t{ s" <tag attr1='value 1' >" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value 1" ?str s" attr1" ?str }t

t{ s" hello<tag attr1='value&#38;1'>bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" ?str }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value&1" ?str s" attr1" ?str }t
t{ xis1 xis-read xis.text          ?s s" bye" ?str }t

t{ s" <tag attr1='value&#381'>" xis1 xis-set-string }t  \ mistake, missing ;

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value&#381" ?str s" attr1" ?str }t

t{ s" <tag attr1='value&#a;1'>" xis1 xis-set-string }t  \ mistake, not numerical reference

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value&#a;1" ?str s" attr1" ?str }t


\ Tag with attribute with double-quoted value

t{ s\" <tag attr1=\"value 1\">" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value 1" ?str s" attr1" ?str }t

t{ s\" <tag attr1 = \"value 1\">" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value 1" ?str s" attr1" ?str }t

t{ s\" <tag attr1=\"value 1\" >" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s" value 1" ?str s" attr1" ?str }t

t{ s\" hello<tag attr1=\"value&quot;1\">bye" xis1 xis-set-string }t

t{ xis1 xis-read xis.text          ?s s" hello" ?str }t
t{ xis1 xis-read xis.start-tag     ?s s" tag" ?str 1 ?s s\" value\"1" ?str s" attr1" ?str }t
t{ xis1 xis-read xis.text          ?s s" bye" ?str }t


\ Combination tag test 

t{ s\" <airplane brand=\"airtrain\" color=brown&amp;yellow type='three wings'/>" xis1 xis-set-string }t

t{ xis1 xis-read xis.empty-element ?s s" airplane" ?str 3 ?s s" three wings" ?str s" type" ?str s" brown&yellow" ?str s" color" ?str s" airtrain" ?str s" brand" ?str }t

\ Error tags

t{ s" <0tag>" xis1 xis-set-string }t

t{ xis1 xis-read xis.error ?s }t

t{ s" <tag attr1='a>" xis1 xis-set-string }t

t{ xis1 xis-read xis.error ?s }t


\ Comment, CDATA, text tests

t{ s" <!--Comment1--><![CDATA[Hallo]]>&gt;Test1&amp;Test2&unknown;Test3&lt;<!--Comment2--></tag ><bold />" xis1 xis-set-string }t 

t{ xis1 xis-read xis.comment       ?s s" Comment1" ?str }t
t{ xis1 xis-read xis.cdata         ?s s" Hallo"    ?str }t
t{ xis1 xis-read xis.text          ?s s" >Test1&Test2&unknown;Test3<" ?str }t
t{ xis1 xis-read xis.comment       ?s s" Comment2" ?str }t
t{ xis1 xis-read xis.end-tag       ?s s" tag"      ?str }t
t{ xis1 xis-read xis.empty-element ?s s" bold"     ?str ?0 }t


\ Proc. instr.

t{ s" <?target attribute='value' ?>" xis1 xis-set-string }t

t{ xis1 xis-read xis.proc-instr ?s s" target" ?str 1 ?s s" value" ?str s" attribute" ?str }t


\ DTD tests

t{ s" <!DOCTYPE email [<!ELEMENT message (to,from,subject,body)>]>" xis1 xis-set-string }t

t{ xis1 xis-read xis.internal-dtd ?s s" email" ?str s" <!ELEMENT message (to,from,subject,body)>" ?str }t

t{ s" <!DOCTYPE email SYSTEM 'file.dtd' [<!ELEMENT message (to,from,subject,body)>] >" xis1 xis-set-string }t

t{ xis1 xis-read xis.system-dtd ?s s" email" ?str s" <!ELEMENT message (to,from,subject,body)>" ?str s" file.dtd" ?str }t

t{ s\" <!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional //EN\">" xis1 xis-set-string }t

t{ xis1 xis-read xis.public-dtd ?s s" HTML" ?str ?0 drop ?0 drop s" -//W3C//DTD HTML 4.01 Transitional //EN" ?str }t

t{ s\" <!DOCTYPE email PUBLIC \"-//W3C//DTD HTML 4.01 Transitional //EN\" 'file.dtd' [<!ELEMENT message (to,from,subject,body)>] >" xis1 xis-set-string }t

t{ xis1 xis-read xis.public-dtd ?s s" email" ?str s" <!ELEMENT message (to,from,subject,body)>" ?str s" file.dtd" ?str s" -//W3C//DTD HTML 4.01 Transitional //EN" ?str }t


\ Error DTDs

t{ s" <!DOCTYPE email [<!ELEMENT message (to,from,subject,body)>>" xis1 xis-set-string }t

t{ xis1 xis-read xis.error ?s }t

t{ s" <!DOCTYPE email SYSTEM 'file.dtd [<!ELEMENT message (to,from,subject,body)>] >" xis1 xis-set-string }t

t{ xis1 xis-read xis.error ?s }t

\ Space stripping tests

t{ s" <bold>   </bold>" xis1 xis-set-string }t

t{ xis1 xis-strip@ ?false }t

t{ xis1 xis-read xis.start-tag ?s s" bold" ?str ?0 }t
t{ xis1 xis-read xis.text      ?s s"    "  ?str    }t
t{ xis1 xis-read xis.end-tag   ?s s" bold" ?str    }t

t{ true xis1 xis-strip!  }t
t{ xis1 xis-strip@ ?true }t


t{ s" <bold>   </bold>" xis1 xis-set-string }t

t{ xis1 xis-read xis.start-tag ?s s" bold" ?str ?0 }t
t{ xis1 xis-read xis.end-tag   ?s s" bold" ?str    }t

\ Entity reference catalog

msc-create xis.msc

 s" copy" s" (c)" xis.msc msc-add   \ Only one item in the catalog

t{ xis.msc xis1 xis-msc! }t

t{ s" FFL &copy; &lt;&#33;&gt;" xis1 xis-set-string }t

t{ xis1 xis-read xis.text ?s s" FFL (c) &lt;!&gt;" ?str }t



\ Reading from a file

t{ xis-new value xis2 }t

: xis-test-reader   ( file-id -- c-addr u | 0 )
  pad 64 rot read-file throw
  dup IF
    pad swap 
  THEN
;

t{ s" test.xml" r/o open-file throw value xis.file }t

t{ xis.file ' xis-test-reader xis2 xis-set-reader }t

t{ true xis2 xis-strip!  }t

t{ xis2 xis-read xis.start-xml ?s 2 ?s s" no" ?str s" standalone" ?str s" 1.0" ?str s" version" ?str }t

t{ xis2 xis-read xis.comment ?s s"  This is a test file for the ffl-library " ?str }t

t{ xis2 xis-read xis.start-tag ?s s" TEST" ?str ?0 }t
t{ xis2 xis-read xis.start-tag ?s s" BOOK" ?str ?0 }t
t{ xis2 xis-read xis.start-tag ?s s" TITLE" ?str ?0 }t
t{ xis2 xis-read xis.text      ?s s" Starting Forth" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" TITLE" ?str }t
t{ xis2 xis-read xis.start-tag ?s s" AUTHOR" ?str ?0 }t
t{ xis2 xis-read xis.start-tag ?s s" NAME" ?str ?0 }t
t{ xis2 xis-read xis.text      ?s s" Leo Brodie" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" NAME" ?str }t
t{ xis2 xis-read xis.empty-element ?s s" FIRSTBOOK" ?str ?0 }t
t{ xis2 xis-read xis.end-tag   ?s s" AUTHOR" ?str }t
t{ xis2 xis-read xis.start-tag ?s s" PUBLISHER" ?str ?0 }t
t{ xis2 xis-read xis.text      ?s s" Prentice-Hall" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" PUBLISHER" ?str }t
t{ xis2 xis-read xis.start-tag ?s s" DATE" ?str ?0 }t
t{ xis2 xis-read xis.text      ?s s\" \"1981\"" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" DATE" ?str }t
t{ xis2 xis-read xis.start-tag ?s s" PAGES" ?str ?0 }t
t{ xis2 xis-read xis.text      ?s s" 235" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" PAGES" ?str }t
t{ xis2 xis-read xis.start-tag ?s s" REVIEW" ?str ?0 }t
t{ xis2 xis-read xis.text      ?s s" This book is the best!." ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" REVIEW" ?str }t
t{ xis2 xis-read xis.empty-element ?s s" AVAILABLE" ?str ?0 }t
t{ xis2 xis-read xis.cdata     ?s s\" Dutch: \"Forth, een praktische introduktie\"" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" BOOK" ?str }t
t{ xis2 xis-read xis.end-tag   ?s s" TEST" ?str }t
t{ xis2 xis-read xis.done      ?s }t

xis.file close-file throw

t{ xis2 xis-free }t


\ Reading from a file

t{ xis-new value xis3 }t

t{ s" test.xml" r/o open-file throw to xis.file }t

t{ xis.file ' xis-test-reader xis3 xis-set-reader }t

t{ true xis3 xis-strip!  }t

t{ xis3 xis-read xis+remove-read-parameters }t

t{ xis3 xis-read xis+remove-read-parameters }t

t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis+remove-read-parameters }t
t{ xis3 xis-read xis.done      ?s }t

xis.file close-file throw

t{ xis3 xis-free }t

\ ==============================================================================
