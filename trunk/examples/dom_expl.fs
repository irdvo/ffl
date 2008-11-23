\ ==============================================================================
\
\               dom_expl - the xml-dom example in the ffl
\
\               Copyright (C) 2008  Dick van Oudheusden
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
\  $Date: 2008-11-23 06:48:53 $ $Revision: 1.5 $
\
\ ==============================================================================

include ffl/dom.fs


\ Example 1: Read xml from file, iterate and write to string


\ Create the xml-dom in the dictionary

dom-create dom1


\ Setup a file reader for the dom

: dom-reader   ( file-id -- c-addr u | 0 = Read the next chunk of the file )
  pad 64 rot read-file throw
  dup IF
    pad swap
  THEN
;


\ Open the source xml file

s" test.xml" r/o open-file throw value dom.input 


\ Read the test.xml file with the dom-reader in the dom, leading and trailing whitespace is skipped

dom.input ' dom-reader true dom1 dom-read-reader [IF]
  .( XML File is successfully read ) cr
[ELSE]
  .( XML File is not correct ) cr
[THEN]


\ Iterate in the dom the xml-document, start with the root

dom1 dom-document [IF]
  .( Iterate the start of the xml document ) cr
[ELSE]
  .( No document start ?? ) cr
[THEN]


\ Move the iterator to the first child of the xml-document

dom1 dom-child [IF]
  dup dom.attribute = [IF]
    drop
    .( Attribute with name: ) dom1 dom-get-name type .(  and value: ) dom1 dom-get-value type cr
  [ELSE]
    dup dom.element = [IF]
      drop
      .( Tag with name: ) dom1 dom-get-name type .(  and value: ) dom1 dom-get-value type cr
    [ELSE]
      dup dom.comment = [IF]
        drop
        .( Comment with value: ) dom1 dom-get-value type cr
      [ELSE]
        dup dom.text = [IF]
          drop
          .( Text with value: ) dom1 dom-get-value type cr
        [ELSE]
          dom.pi = [IF]
            .( Processing instruction with name: ) dom1 dom-get-name type .(  and value: ) dom1 dom-get-value type cr
          [ELSE]
            .( Perhaps a CDATA section ?) cr
          [THEN]
        [THEN]
      [THEN]
    [THEN]
  [THEN]
[ELSE]
  .( xml document has no children.) cr
[THEN]


\ Move the iterator to the next child of the xml-document

dom1 dom-next [IF]
  dup dom.attribute = [IF]
    drop
    .( Attribute with name: ) dom1 dom-get-name type .(  and value: ) dom1 dom-get-value type cr
  [ELSE]
    dup dom.element = [IF]
      drop
      .( Tag with name: ) dom1 dom-get-name type .(  and value: ) dom1 dom-get-value type cr
    [ELSE]
      dup dom.comment = [IF]
        drop
        .( Comment with value: ) dom1 dom-get-value type cr
      [ELSE]
        dup dom.text = [IF]
          drop
          .( Text with value: ) dom1 dom-get-value type cr
        [ELSE]
          dom.pi = [IF]
            .( Processing instruction with name: ) dom1 dom-get-name type .(  and value: ) dom1 dom-get-value type cr
          [ELSE]
            .( Perhaps a CDATA section ?) cr
          [THEN]
        [THEN]
      [THEN]
    [THEN]
  [THEN]
[ELSE]
  .( xml document has no more children.) cr
[THEN]


\ Write the xml

dom1 dom-write-string [IF]
  .( xml document: ) type cr
[ELSE]
  .( Problems writing the xml document.)  cr
[THEN]
[THEN]



\ Example 2:  Read xml from string and write to a file


\ Create the xml-dom on the heap

dom-new value dom2


\ Read xml from a string, skipping any leading and trailing whitespace

s" <?xml version='1.1'?>  <!-- test -->  <car>  <color>  blue  </color>  </car>" true dom2 dom-read-string [IF] 
  .( XML is sucessfully read.) cr
[ELSE]
  .( XML was not correct.) cr
[THEN]


\ Write the xml-dom to a file using a writer

: dom-writer  ( c-addr u file-id -- flag = Write the xml using a writer )
  write-file throw
  true
;


\ Open the file for the writer

s" out.xml" w/o create-file throw value dom.output


\ Write the xml-dom to the writer

dom.output ' dom-writer dom2 dom-write-writer [IF]
  .( XML is successfully written.) cr
[ELSE]
  .( Problems writing the xml-dom.) cr
[THEN]


\ Free the dom from the heap

dom2 dom-free


\ Example 3: build a xml document from scratch using the xml-dom


\ Create the xml-dom on the heap

dom-new value dom3


\ Start with the root, the xml-document

dom.document dom3 dom-append-node


\ Add the version attribute to the xml-document

s" version" s" 1.0" dom.attribute dom3 dom-append-node


\ Move back to the xml-document and add a tag

dom3 dom-parent 2drop
s" tag" dom.element dom3 dom-append-node


\ Add text to the element

s" hello" dom.text dom3 dom-append-node


\ Write the xml to a string

dom3 dom-write-string [IF]
  .( XML successfully written: ) type cr
[ELSE]
  .( Problems...) cr
[THEN]


\ Free the dom from the heap

dom3 dom-free
