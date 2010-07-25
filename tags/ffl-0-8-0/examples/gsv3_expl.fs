\ ==============================================================================
\
\        gsv3_expl - the gtk-server box packing example in the ffl
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

\ Load all definitions from the gtk-server.cfg file

s" gtk-server.cfg" s" ffl-fifo" gsv+open 0= [IF]


  : event>widget   ( c-addr u -- n = Convert the event string to a widget id )
    0. 2swap >number 2drop d>s
  ;


  : bool>string  ( f -- c-addr u = Convert a flag to a string )
    IF s" TRUE" ELSE s" FALSE" THEN
  ;


  : new-button   ( u f1 f2 hbox c-addr u -- Create a new button with label c-addr u in the hbox with expand f2, fill f1 and padding u )
  
    gtk_button_new_with_label >r
    
    r@ swap gtk_box_pack_start
    
    r> gtk_widget_show
  ;


  : make-box     ( u f1 f2 n f3 -- hbox = Create a hbox with buttons with homogeneous f3, spacing n, expand f2, fill f1 and padding u )
    gtk_hbox_new                  \ Create a hbox with homogeneous and spacing
    
    2over 2over  2>r over 0 <# #s #> 2r> 2swap  new-button  \ Create button with padding
    
    2over 2over   >r over bool>string r> -rot   new-button  \ Create button with fill
    
    2over 2over      over bool>string           new-button  \ Create button with expand

    2over 2over                s" button"       new-button

    2over 2over                   s" box"       new-button

    2over 2over          s" gtk_box_pack"       new-button
    
    nip nip nip
  ;
  
  
  : new-label  ( c-addr u vbox -- = Create a new label with label c-addr u in vbox )
    >r
    gtk_label_new >r
    
    0E+0 0E+0 r@ gtk_misc_set_alignment
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
    
    rdrop
  ;
  
  
  : new-separator  ( n f1 f2 vbox -- = Create a new separator with padding n fill f1 and expand f2 )
  
    gtk_hseparator_new >r
    
    r@ swap gtk_box_pack_start
    
    r> gtk_widget_show
  ;


  0 value window1                 \ the event widgets
  0 value exit-button1
  0 value window2
  0 value exit-button2
  0 value window3
  0 value exit-button3


  : example1
                                  \ the top level window  
      GTK_WINDOW_TOPLEVEL gtk_window_new to window1
       
    10 window1 gtk_container_set_border_width

    s" Packbox" window1 gtk_window_set_title


    0 false gtk_vbox_new >r 
    
    s" 0 false gtk_hbox_new" r@ new-label
    

    0 false false 0 false make-box >r \ homogeneous = FALSE, spacing = 0, expand = FALSE, fill = FALSE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
	
    r> gtk_widget_show


    0 false true 0 false make-box >r  \ homogeneous = FALSE, spacing = 0, expand = TRUE, fill = FALSE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    0 true true 0 false make-box >r  \ homogeneous = FALSE, spacing = 0, expand = TRUE, fill = TRUE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    5 false false r@ new-separator
    
    s" 0 true gtk_hbox_new" r@ new-label

    0 false true 0 true make-box >r  \ homogeneous = TRUE, spacing = 0, expand = FALSE, fill = TRUE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show


    0 true true 0 true make-box >r  \ homogeneous = TRUE, spacing = 0, expand = TRUE, fill = TRUE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show

    5 true false r@ new-separator
    
    
    0 false gtk_hbox_new >r
    
    s" Quit" gtk_button_new_with_label to exit-button1
     
    0 false true exit-button1 r@ gtk_box_pack_start
    
    exit-button1 gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show
    
    r@ window1 gtk_container_add
    
    r> gtk_widget_show
    
    window1 gtk_widget_show
  ;
  
  
  : example2
    GTK_WINDOW_TOPLEVEL gtk_window_new to window2
       
    10 window2 gtk_container_set_border_width

    s" Packbox" window2 gtk_window_set_title


    0 false gtk_vbox_new >r


    s" 10 false gtk_hbox_new" r@ new-label

    0 false true 10 false make-box >r \ homogeneous = FALSE, spacing = 10, expand = TRUE, fill = FALSE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
	
    r> gtk_widget_show


    0 true true 10 false make-box >r \ homogeneous = FALSE, spacing = 10, expand = TRUE, fill = TRUE, padding = 0
    
    0 false false r@ r'@ gtk_box_pack_start
	
    r> gtk_widget_show

    
    5 true false r@ new-separator
    
    s" 0 false gtk_hbox_new" r@ new-label
    
    
    10 false true 0 false make-box >r \ homogeneous = FALSE, spacing = 0, expand = TRUE, fill = FALSE, padding = 10
    
    0 false false r@ r'@ gtk_box_pack_start
	
    r> gtk_widget_show
    

    10 true true 0 false make-box >r \ homogeneous = FALSE, spacing = 0, expand = TRUE, fill = TRUE, padding = 10
    
    0 false false r@ r'@ gtk_box_pack_start
	
    r> gtk_widget_show

 	
    5 true false r@ new-separator

    0 false gtk_hbox_new >r
    
    s" Quit" gtk_button_new_with_label to exit-button2
    
    0 false true exit-button2 r@ gtk_box_pack_start
    
    exit-button2 gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show

    r@ window2 gtk_container_add
    
    r> gtk_widget_show
    
    window2 gtk_widget_show
  ;
  

  : example3

    GTK_WINDOW_TOPLEVEL gtk_window_new to window3
      
    10 window3 gtk_container_set_border_width

    s" Packbox" window3 gtk_window_set_title


    0 false gtk_vbox_new >r

    0 false false 0 false make-box >r \ homogeneous = FALSE, spacing = 0, expand = FALSE, fill = FALSE, padding = 10

    
    s" end" gtk_label_new >r
    
    0 false false r@ r'@ gtk_box_pack_end
    
    r> gtk_widget_show
    
    
    0 false false r@ r'@ gtk_box_pack_start

    r> gtk_widget_show


    gtk_hseparator_new >r
    
    5 400 r@ gtk_widget_set_size_request
    
    5 true false r@ r'@ gtk_box_pack_start

    r> gtk_widget_show
    

    5 true false r@ new-separator


    0 false gtk_hbox_new >r
    
    s" Quit" gtk_button_new_with_label to exit-button3
    
    0 false true exit-button3 r@ gtk_box_pack_start
    
    exit-button3 gtk_widget_show
    
    0 false false r@ r'@ gtk_box_pack_start
    
    r> gtk_widget_show

    r@ window3 gtk_container_add
    
    r> gtk_widget_show
    
    window3 gtk_widget_show
  ;
  
  
  : example
    gtk_init        \ Initialise toolkit
    
    example1    \ Show three toplevel windows
    example2
    example3
                                \ Main loop
    BEGIN
      s" WAIT" gtk_server_callback
      event>widget CASE
        window1      OF true ENDOF
        exit-button1 OF true ENDOF
        window2      OF true ENDOF
        exit-button2 OF true ENDOF
        window3      OF true ENDOF
        exit-button3 OF true ENDOF
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
