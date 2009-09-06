\ ==============================================================================
\
\     gsv9_expl - the gtk-server arrow / tooltip example in the ffl
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

include ffl/gsv.fs

\ Open the connection to the gtk-server and load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  : create_arrow_button  ( n1 n2 -- widget = Create arrow button with shadow type n1 and array type n2 )
    gtk_button_new >r
    
    gtk_arrow_new >r 

    r@ r'@ gtk_container_add
  
    r> gtk_widget_show
    r@ gtk_widget_show

    r>
  ;


  0 value window                  \ the widget events


  : example
    gtk_init                    \ Initialise toolkit
    
    GTK_WINDOW_TOPLEVEL gtk_window_new to window
    
    s" Arrow Buttons" window gtk_window_set_title

    \ Sets the border width of the window.
    10 window gtk_container_set_border_width

    \ Create a box to hold the arrows/but<tons
    0 false gtk_hbox_new >r
    
    2 r@ gtk_container_set_border_width
    
    r@ window gtk_container_add


    
    GTK_SHADOW_IN GTK_ARROW_UP create_arrow_button >r
        
    gtk_tooltips_new >r
    s" " s" Arrow button1" r'@ r> gtk_tooltips_set_tip 

    3 false false r> r@ gtk_box_pack_start

    GTK_SHADOW_OUT GTK_ARROW_DOWN create_arrow_button >r
    3 false false r> r@ gtk_box_pack_start
  
    GTK_SHADOW_ETCHED_IN GTK_ARROW_LEFT create_arrow_button >r
    3 false false r> r@ gtk_box_pack_start
  
    GTK_SHADOW_ETCHED_OUT GTK_ARROW_RIGHT create_arrow_button >r
    3 false false r> r@ gtk_box_pack_start
  
    \ Pack and show all our widgets
    r>     gtk_widget_show
    window gtk_widget_show
 
                                  \ main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window       OF true ENDOF
        false swap
      ENDCASE
    UNTIL
    
    0 gtk_exit
  ;

  example
  
  gsv+close drop
    
[ELSE]
  .( No gtk-server fifo, is the gtk-server running ? ) cr
[THEN]


\ ==============================================================================
