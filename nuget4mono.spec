%define __jar_repack 0
%define __os_install_post %{nil}
%define __jar_repack 0
Name: nuget4mono
Version: 0.7.0
Release: %{_release}
Summary: nuget4mono
License: (c) 2011 Terradue s.r.l.
Distribution: Terradue 2019
Vendor: Terradue s.r.l.
URL: http://www.terradue.com/
Group: scoop
Packager: Terradue
Provides: nuget4mono
Requires: mono >= 4
autoprov: yes
autoreq: no
BuildArch: noarch

%description
A tool to help using nuget with projects developed with Mono

%install
mkdir -p %{buildroot}/usr/lib/nuget4mono
cp -r %{_sourcedir}/bin/* %{buildroot}/usr/lib/nuget4mono
mkdir -p %{buildroot}/usr/bin/
cp %{_sourcedir}/nuget4mono %{buildroot}/usr/bin/

%files
%defattr(644,root,root,755)
%attr(755,root,root)  "/usr/bin/nuget4mono"
%attr(755,root,root) "/usr/lib/nuget4mono"
