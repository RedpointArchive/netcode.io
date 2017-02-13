%module netcodeNATIVE
%{
#include "../netcode.io-import/netcode.h"
%}

%include <typemaps.i>

%apply int *OUTPUT { int * packet_bytes };

%typemap(ctype) uint8_t * "uint8_t *" 
%typemap(imtype) uint8_t * "byte[]" 
%typemap(cstype) uint8_t * "byte[]" 
%typemap(in) uint8_t * %{ $1 = ($1_ltype)$input; %} 
%typemap(csin) uint8_t * "$csinput" 

%typemap(ctype)  void * "void *"
%typemap(imtype) void * "System.IntPtr"
%typemap(cstype) void * "System.IntPtr"
%typemap(csin)   void * "$csinput"
%typemap(in)     void * %{ $1 = $input; %}
%typemap(out)    void * %{ $result = $1; %}
%typemap(csout)  void * { return $imcall; }

%typemap(ctype) uint64_t "uint64_t" 
%typemap(imtype) uint64_t "System.UInt64" 
%typemap(cstype) uint64_t "System.UInt64" 
%typemap(in) uint64_t %{ $1 = ($1_ltype)$input; %} 
%typemap(csin) uint64_t "$csinput" 

%include "../netcode.io-import/netcode.h"